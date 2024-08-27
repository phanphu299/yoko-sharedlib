using AHI.Infrastructure.AzureMqttTriggerExtension.Config;

namespace AHI.Infrastructure.AzureMqttTriggerExtension.Bindings
{
    public interface IMqttConfigurationParser
    {
        MqttConfiguration Parse(MqttBaseAttribute mqttAttribute);
    }
}
