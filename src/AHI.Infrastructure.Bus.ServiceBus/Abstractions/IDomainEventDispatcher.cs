using System.Threading.Tasks;

namespace AHI.Infrastructure.Bus.ServiceBus.Abstraction
{
    public interface IDomainEventDispatcher
    {
        Task SendAsync<T>(T message) where T : BusEvent;
    }
}