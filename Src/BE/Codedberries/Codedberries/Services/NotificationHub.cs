using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Codedberries.Services
{
    public class NotificationHub:Hub<INotificationClient>
    {
        public static ConcurrentDictionary<string, List<string>> UserConnections = new ConcurrentDictionary<string, List<string>>();

    }
}
