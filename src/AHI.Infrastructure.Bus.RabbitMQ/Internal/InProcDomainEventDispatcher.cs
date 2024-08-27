using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;

namespace AHI.Infrastructure.Bus.ServiceBus.Internal
{
    internal class InProcDomainEventDispatcher : IDomainEventDispatcher
    {

        public InProcDomainEventDispatcher()
        {
        }

        public Task SendAsync<T>(T message, string routingKey = "all") where T : BusEvent
        {
            return Task.CompletedTask;
        }

        public Task SendAsync<T>(List<T> messages, string routingKey = "all") where T : BusEvent
        {
            return Task.CompletedTask;
        }
    }
}