using System;
using Microsoft.Azure.WebJobs.Description;

namespace AHI.Infrastructure.AzureCoapTriggerExtension
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class CoapTriggerAttribute : Attribute
    {
        public string ConnectionString => Environment.GetEnvironmentVariable("CoapConnection");

        [AutoResolve]
        public string TopicName { get; set; }

        public CoapTriggerAttribute(string topicName)
        {
            TopicName = topicName;
        }
    }
}
