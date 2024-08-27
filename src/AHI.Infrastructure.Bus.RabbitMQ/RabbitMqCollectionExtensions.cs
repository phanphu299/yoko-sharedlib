using System;
using System.Linq;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Exception;
using AHI.Infrastructure.Bus.ServiceBus.Model;
using AHI.Infrastructure.Bus.ServiceBus.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AHI.Infrastructure.Bus.ServiceBus.Extension
{
    /// <summary>
    /// DI extensions for RabbitMQ client (RabbitMQ connection).
    /// </summary>
    public static class RabbitMqCollectionExtensions
    {
        /// <summary>
        /// Add a singleton producing RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="rabbitMqClientConfig">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducingClientSingleton(this IServiceCollection services, RabbitMqClientOptions rabbitMqClientConfig)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IProducingService>();
            services.AddRabbitMqClientInfrastructure();

            var containerId = Guid.NewGuid();
            services.ConfigureRabbitMqProducingClientOptions(containerId, rabbitMqClientConfig);
            services.ResolveSingletonProducingService(containerId);

            return services;
        }

        private static IServiceCollection AddRabbitMqClientInfrastructure(this IServiceCollection services)
        {
            services.AddOptions();
            services.TryAddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            return services;
        }

        private static IServiceCollection ResolveSingletonProducingService(this IServiceCollection services, Guid containerId)
        {
            services.TryAddSingleton<IProducingService>(provider => new QueueService(
                containerId,
                provider.GetService<ILogger<QueueService>>(),
                provider.GetService<IRabbitMqConnectionFactory>(),
                provider.GetServices<RabbitMqConnectionOptionsContainer>()));
            return services;
        }

        private static IServiceCollection CheckIfQueueingServiceAlreadyConfigured<T>(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(T));
            if (descriptor != null)
            {
                throw new QueueingServiceAlreadyConfiguredException(typeof(T), $"A queuing service of type {typeof(T)} has already been configured.");
            }
            return services;
        }

        private static IServiceCollection ConfigureRabbitMqProducingClientOptions(this IServiceCollection services, Guid containerId, RabbitMqClientOptions options)
        {
            var container = new RabbitMqConnectionOptionsContainer
            {
                ContainerId = containerId,
                Options = new RabbitMqConnectionOptions { ProducerOptions = options }
            };
            return services.AddRabbitMqConnectionOptionsContainer(container);
        }

        private static IServiceCollection AddRabbitMqConnectionOptionsContainer(this IServiceCollection services, RabbitMqConnectionOptionsContainer container)
        {
            var serviceDescriptor = new ServiceDescriptor(typeof(RabbitMqConnectionOptionsContainer), container);
            services.Add(serviceDescriptor);
            return services;
        }
    }
}