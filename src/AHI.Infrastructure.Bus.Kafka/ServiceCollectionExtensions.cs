using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Bus.Kafka.Service;
using Confluent.Kafka;
using AHI.Infrastructure.Bus.Kafka.Model;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;

namespace AHI.Infrastructure.Bus.ServiceBus.Extension
{
    public static class ServiceCollectionExtensions
    {
        private const int DEFAULT_START_UP_DELAY = 30;
        public static IServiceCollection AddKafka(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            var kafkaOption = configuration.GetSection(nameof(KafkaOption)).Get<KafkaOption>();

            if (kafkaOption.Enabled.HasValue && kafkaOption.Enabled.Value)
            {
                services.AddSingleton<IProducer<Null, byte[]>>(sp =>
                            {
                                var config = new ProducerConfig
                                {
                                    BootstrapServers = kafkaOption.BootstrapServers,
                                    Acks = kafkaOption?.Producer?.AckMode ?? Acks.Leader,
                                    LingerMs = kafkaOption?.Producer?.Linger ?? 100,
                                };
                                return new ProducerBuilder<Null, byte[]>(config).Build();
                            });
                services.AddSingleton<IDomainEventDispatcher, KafkaPublisher>();
            }
            return services;
        }
    }
}