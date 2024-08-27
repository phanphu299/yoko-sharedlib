using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace AHI.Infrastructure.AzureMqttTriggerExtension.Listeners
{
    public class ManagedMqttClientFactory : IManagedMqttClientFactory
    {
        private readonly IMqttFactory _mqttFactory;

        public ManagedMqttClientFactory(IMqttFactory mqttFactory)
        {
            _mqttFactory = mqttFactory;
        }

        public IManagedMqttClient CreateManagedMqttClient()
        {
            return _mqttFactory.CreateManagedMqttClient();
        }
    }
}
