using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Codedberries.Services
{
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly UserService _userService;

        public static ConcurrentDictionary<string, List<string>> UserConnections = new ConcurrentDictionary<string, List<string>>();

        public NotificationHub(UserService userService)
        {
            _userService = userService;
        }

        public override async System.Threading.Tasks.Task OnConnectedAsync()
        {
            Trace.TraceInformation("MapHub started. ID: {0}", Context.ConnectionId);

            // Extract session token from request cookies
            var httpContext = Context.GetHttpContext();
            string? sessionToken = httpContext.Request.Cookies["sessionId"];

            if (!string.IsNullOrEmpty(sessionToken))
            {
                // Validate session token and get username
                var username = _userService.GetUsernameFromSessionToken(sessionToken);

                if (username != null)
                {
                    // Add connection ID to the user's list of connections
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
                    Console.WriteLine("Failed to extract username from session token.");
                }
            }
            else
            {
                Console.WriteLine("Session token not found in request cookies.");
            }

            await base.OnConnectedAsync();
        }

        public override async System.Threading.Tasks.Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            string? sessionToken = httpContext.Request.Cookies["sessionId"];

            if (!string.IsNullOrEmpty(sessionToken))
            {
                var username = _userService.GetUsernameFromSessionToken(sessionToken);

                if (username != null)
                {
                    List<string> existingUserConnectionIds;
                    UserConnections.TryGetValue(username, out existingUserConnectionIds);

                    existingUserConnectionIds?.Remove(Context.ConnectionId);

                    if (existingUserConnectionIds?.Count == 0)
                    {
                        List<string> garbage;
                        UserConnections.TryRemove(username, out garbage);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to extract username from session token.");
                }
            }
            else
            {
                Console.WriteLine("Session token not found in request cookies.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async System.Threading.Tasks.Task SendNotificationToUser(string username, NotificationDTO message)
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
                Console.WriteLine($"User {username} is not connected or has no active connections.");
            }
        }
    }
}
