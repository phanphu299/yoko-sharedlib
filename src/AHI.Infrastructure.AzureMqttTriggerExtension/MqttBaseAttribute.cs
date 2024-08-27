using System;

namespace AHI.Infrastructure.AzureMqttTriggerExtension
{
    public abstract class MqttBaseAttribute : Attribute
    {
        protected MqttBaseAttribute()
        {
        }

        protected MqttBaseAttribute(Type mqttConfigCreatorType)
        {
            MqttConfigCreatorType = mqttConfigCreatorType;
        }

        public string ConnectionString { get; set; }

        public Type MqttConfigCreatorType { get; protected set; }
    }
}