using MQTTnet.Extensions.ManagedClient;

namespace AHI.Infrastructure.AzureMqttTriggerExtension.Listeners
{
    public interface IManagedMqttClientFactory
    {
        IManagedMqttClient CreateManagedMqttClient();
    }
}
