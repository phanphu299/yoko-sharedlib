using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using Newtonsoft.Json;

namespace AHI.Infrastructure.Bus.ServiceBus.Internal
{
    internal class OutProcDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly ServiceBusClient _serviceBusClient;

        public OutProcDomainEventDispatcher(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }
        public Task SendAsync<T>(T message) where T : BusEvent
        {
            var busSender = _serviceBusClient.CreateSender(message.TopicName);
            return busSender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(new { Message = message })));
        }
    }
}
