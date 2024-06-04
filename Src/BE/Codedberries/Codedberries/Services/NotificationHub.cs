using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace Codedberries.Services
{
    public class NotificationHub:Hub<INotificationClient>
    {
        private readonly TokenService _tokenService;
        public static ConcurrentDictionary<string, List<string>> UserConnections = new ConcurrentDictionary<string, List<string>>();
        public NotificationHub(TokenService tokenService)
        {
            _tokenService = tokenService;
        }
       
        public override async Task OnConnectedAsync()
        {
            Trace.TraceInformation("MapHub started. ID: {0}", Context.ConnectionId);
            var httpContext = Context.GetHttpContext();
            var token = httpContext.Request.Query["access_token"].FirstOrDefault();
            var username = _tokenService.GetUsernameFromToken(token) ?? throw new Exception("Korisničko ime nije pronađeno u JWT tokenu.");

            if (username != null)
            {
                List<string> existingUserConnectionIds;
                if (!UserConnections.TryGetValue(username, out existingUserConnectionIds))
                {
                    existingUserConnectionIds = new List<string>();
                    UserConnections.TryAdd(username, existingUserConnectionIds);
                }

                existingUserConnectionIds.Add(Context.ConnectionId);
            }
            else
            {
                Console.WriteLine("Failed to extract username from JWT token.");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var token = httpContext.Request.Query["access_token"].FirstOrDefault();
            var username = _tokenService.GetUsernameFromToken(token) ?? throw new Exception("Korisničko ime nije pronađeno u JWT tokenu.");

            List<string> existingUserConnectionIds;
            UserConnections.TryGetValue(username, out existingUserConnectionIds);

            existingUserConnectionIds.Remove(Context.ConnectionId);

            if (existingUserConnectionIds.Count == 0)
            {
                List<string> garbage;
                UserConnections.TryRemove(username, out garbage);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string username, string message)
        {
            if (UserConnections.TryGetValue(username, out List<string> connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).ReceiveNotification(message);
                }
            }
            else
            {
                Console.WriteLine($"Korisnik {username} nije povezan ili nema aktivnih konekcija.");
            }
        }


    }
}
 