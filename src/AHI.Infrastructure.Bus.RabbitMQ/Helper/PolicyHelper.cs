using System.Threading.Tasks;
using Polly;
using RabbitMQ.Client.Exceptions;
using System;

namespace AHI.Infrastructure.Bus.ServiceBus.Helpers
{
    public class PolicyHelper
    {
        public static Policy CreatePolicy()
        {
            return Policy
           .Handle<BrokerUnreachableException>()
           .WaitAndRetry(new[]
           {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(8),
                    TimeSpan.FromSeconds(13)
           });
        }
    }
}