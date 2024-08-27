using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Model;

namespace AHI.Infrastructure.Audit.Service.Abstraction
{
    public interface INotificationService
    {
        Task SendNotifyAsync(string endpoint, NotificationMessage message);
    }
}