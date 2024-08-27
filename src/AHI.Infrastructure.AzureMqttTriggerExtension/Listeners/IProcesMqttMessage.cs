using System.Threading.Tasks;

namespace AHI.Infrastructure.AzureMqttTriggerExtension.Listeners
{
    public interface IProcesMqttMessage
    {
        Task OnMessage(MqttMessageReceivedEventArgs arg);
    }
}
