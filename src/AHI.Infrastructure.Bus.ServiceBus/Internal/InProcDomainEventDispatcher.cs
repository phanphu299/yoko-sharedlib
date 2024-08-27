using System.Threading.Tasks;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;

namespace AHI.Infrastructure.Bus.ServiceBus.Internal
{
    internal class InProcDomainEventDispatcher : IDomainEventDispatcher
    {

        public InProcDomainEventDispatcher()
        {
        }
        public Task SendAsync<T>(T message) where T : BusEvent
        {
            return Task.CompletedTask;
        }
    }
}
