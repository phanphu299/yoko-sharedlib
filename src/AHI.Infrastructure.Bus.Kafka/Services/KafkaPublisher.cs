using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Helpers;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Confluent.Kafka;
using Polly;

namespace AHI.Infrastructure.Bus.Kafka.Service
{
    public class KafkaPublisher : IDomainEventDispatcher
    {
        private readonly IProducer<Null, byte[]> _publisher;
        private readonly ILoggerAdapter<KafkaPublisher> _logger;

        public KafkaPublisher(IProducer<Null, byte[]> publisher, ILoggerAdapter<KafkaPublisher> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public Task SendAsync<T>(T message, string routingKey = "all") where T : BusEvent
        {
            var policy = PolicyHelper.CreatePolicy();
            SendAsync(policy, message, routingKey);

            return Task.CompletedTask;
        }

        public Task SendAsync<T>(List<T> messages, string routingKey = "all") where T : BusEvent
        {
            var policy = PolicyHelper.CreatePolicy();

            foreach (var message in messages)
            {
                SendAsync(policy, message, routingKey);
            }
            
            return Task.CompletedTask;
        }

        private Task SendAsync<T>(Policy policy, T message, string routingKey) where T : BusEvent
        {
            policy.Execute(() =>
            {
                _publisher.Produce(message.TopicName, new Message<Null, byte[]>()
                {
                    Value = new { Message = message }.Serialize()
                }, HandleReport); 
            });
            return Task.CompletedTask;
        }

        private void HandleReport(DeliveryReport<Null, byte[]> report)
        {
            if(report.Error != null && report.Error.IsError)
            {
                _logger.LogError("[{time}] Handle report: Topic={topic}, error={error}", report.Timestamp.UnixTimestampMs, report.Topic, report.Error.ToJson());
            }
            else 
            {
                _logger.LogInformation("[{time}] Handle report: Topic={topic}", report.Timestamp.UtcDateTime, report.Topic);
            }
        }
    }
}