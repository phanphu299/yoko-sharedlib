using System;
using AHI.Infrastructure.AzureMqttTriggerExtension.Messaging;

namespace AHI.Infrastructure.AzureMqttTriggerExtension.Listeners
{
    public sealed class MqttMessageReceivedEventArgs : EventArgs
    {
        public MqttMessageReceivedEventArgs(IMqttMessage message)
        {
            Message = message;
        }

        public IMqttMessage Message { get; }
    }
}
