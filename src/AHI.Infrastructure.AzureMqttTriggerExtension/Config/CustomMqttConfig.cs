using MQTTnet.Extensions.ManagedClient;

namespace AHI.Infrastructure.AzureMqttTriggerExtension.Config
{
    /// <summary>
    /// Configuration for the MQTT binding.
    /// </summary>
    public abstract class CustomMqttConfig
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        public abstract IManagedMqttClientOptions Options { get; }

        public abstract string Name { get; }
    }
}
