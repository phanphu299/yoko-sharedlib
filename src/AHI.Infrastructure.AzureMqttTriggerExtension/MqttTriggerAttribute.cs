﻿using Microsoft.Azure.WebJobs.Description;
using MQTTnet;
using MQTTnet.Protocol;
using System;

namespace AHI.Infrastructure.AzureMqttTriggerExtension
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class MqttTriggerAttribute : MqttBaseAttribute
    {
        public MqttTriggerAttribute()
        {
        }

        public MqttTriggerAttribute(params string[] topics)
        {
            TopicStrings = topics;
        }

        [AutoResolve]
        public string MqttTopic { get; set; }

        public MqttTriggerAttribute(string topic, Messaging.MqttQualityOfServiceLevel qos = Messaging.MqttQualityOfServiceLevel.AtLeastOnce, Messaging.NoLocal noLocal = Messaging.NoLocal.NotSet, Messaging.RetainAsPublished retainAsPublished = Messaging.RetainAsPublished.NotSet, Messaging.MqttRetainHandling retainHandling = Messaging.MqttRetainHandling.NotSet)
        {
            MqttTopic = topic;
            TopicFilter = new TopicFilter
            {
                Topic = MqttTopic,
                NoLocal = noLocal switch
                {
                    Messaging.NoLocal.True => true,
                    Messaging.NoLocal.False => false,
                    Messaging.NoLocal.NotSet => null,
                    _ => null
                },
                QualityOfServiceLevel = qos switch
                {
                    Messaging.MqttQualityOfServiceLevel.AtLeastOnce => MqttQualityOfServiceLevel.AtLeastOnce,
                    Messaging.MqttQualityOfServiceLevel.AtMostOnce => MqttQualityOfServiceLevel.AtMostOnce,
                    Messaging.MqttQualityOfServiceLevel.ExactlyOnce => MqttQualityOfServiceLevel.ExactlyOnce,
                    _ => MqttQualityOfServiceLevel.AtLeastOnce
                },
                RetainAsPublished = retainAsPublished switch
                {
                    Messaging.RetainAsPublished.True => true,
                    Messaging.RetainAsPublished.False => false,
                    Messaging.RetainAsPublished.NotSet => null,
                    _ => null
                },
                RetainHandling = retainHandling switch
                {
                    Messaging.MqttRetainHandling.DoNotSendOnSubscribe => MqttRetainHandling.DoNotSendOnSubscribe,
                    Messaging.MqttRetainHandling.SendAtSubscribe => MqttRetainHandling.SendAtSubscribe,
                    Messaging.MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly => MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly,
                    Messaging.MqttRetainHandling.NotSet => null,
                    _ => null,
                }
            };
        }

        public MqttTriggerAttribute(Type mqttConfigCreatorType, params string[] topics) : base(mqttConfigCreatorType)
        {
            TopicStrings = topics;
        }

        public TopicFilter TopicFilter { get; }

        internal string[] TopicStrings { get; }
    }
}
