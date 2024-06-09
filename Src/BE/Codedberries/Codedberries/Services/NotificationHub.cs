using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Codedberries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Codedberries.Services
{
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly UserService _userService;
        private readonly AuthorizationService _authorizationService;

        public static ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        public NotificationHub(UserService userService, AuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        public override async System.Threading.Tasks.Task OnConnectedAsync()
        {
            Trace.TraceInformation("MapHub started. ID: {0}", Context.ConnectionId);

            // Extract session token from request cookies
            var httpContext = Context.GetHttpContext();
            var userId = _authorizationService.GetUserIdFromSession(httpContext).ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.TryAdd(userId, Context.ConnectionId);
            }
            else
            {
                Console.WriteLine("Failed to extract user ID from session token or session token not found.");
            }

            await base.OnConnectedAsync();
        }

        public override async System.Threading.Tasks.Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = _authorizationService.GetUserIdFromSession(httpContext).ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                string garbage;
                UserConnections.TryRemove(userId, out garbage);
            }
            else
            {
                Console.WriteLine("Failed to extract user ID from session token or session token not found.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async System.Threading.Tasks.Task SendNotificationToUser(string userId, NotificationDTO message)
        {
            if (UserConnections.TryGetValue(userId, out string connectionId))
            {
                await Clients.Client(connectionId).ReceiveNotification(message);
            }
            else
            {
                Console.WriteLine($"User {userId} is not connected or has no active connections.");
            }
        }
    }
}
