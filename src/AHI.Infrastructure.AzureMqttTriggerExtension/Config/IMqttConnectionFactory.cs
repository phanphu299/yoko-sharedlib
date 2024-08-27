using System.Threading.Tasks;
using AHI.Infrastructure.AzureMqttTriggerExtension.Listeners;

namespace AHI.Infrastructure.AzureMqttTriggerExtension.Config
{
    public interface IMqttConnectionFactory
    {
        Task DisconnectAll();

        MqttConnection GetMqttConnection(MqttBaseAttribute attribute);
    }
}
