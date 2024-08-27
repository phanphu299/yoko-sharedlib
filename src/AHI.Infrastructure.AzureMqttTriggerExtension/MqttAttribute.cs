using Microsoft.Azure.WebJobs.Description;
using System;

namespace AHI.Infrastructure.AzureMqttTriggerExtension
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class MqttAttribute : MqttBaseAttribute
    {
        public MqttAttribute()
        {
        }

        public MqttAttribute(Type mqttConfigCreatorType) : base(mqttConfigCreatorType)
        {
        }
    }
}
