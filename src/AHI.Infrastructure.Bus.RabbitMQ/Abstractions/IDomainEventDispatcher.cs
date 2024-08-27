using System.Collections.Generic;
using System.Threading.Tasks;

namespace AHI.Infrastructure.Bus.ServiceBus.Abstraction
{
    public interface IDomainEventDispatcher
    {
        Task SendAsync<T>(T message, string routingKey = "all") where T : BusEvent;
        Task SendAsync<T>(List<T> messages, string routingKey = "all") where T : BusEvent;
    }
}