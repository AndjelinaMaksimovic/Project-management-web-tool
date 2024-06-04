using Codedberries.Models;

namespace Codedberries.Services
{
    public interface INotificationClient
    {
        System.Threading.Tasks.Task ReceiveNotification(string notification);
    }
}
