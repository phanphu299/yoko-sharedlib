using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Bus.ServiceBus.Internal;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Option;
using AHI.Infrastructure.Bus.ServiceBus.Constant;

namespace AHI.Infrastructure.Bus.ServiceBus.Extension
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add singleton RabbitMQ (This is a customization from https://github.com/antonyvorontsov/RabbitMQ.Client.Core.DependencyInjection to make it fit with the project)
        /// </summary>
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, string clientProvidedName = null)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            var rabbitMQConfig = new RabbitMQOptions(clientProvidedName);

            configuration.GetSection(SectionName.AZURE_SERVICE_BUS).Bind(rabbitMQConfig);

            var rabbitMqClientConfig = rabbitMQConfig.GetRabbitMqClientOptions();

            services.AddRabbitMqProducingClientSingleton(rabbitMqClientConfig);
            services.AddSingleton<IDomainEventDispatcher, RabbitMQOutProcEventDispatcher>();

            return services;
        }

        public static IServiceCollection AddServiceBus(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMQOptions>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                var option = new RabbitMQOptions();
                configuration.GetSection("AzureServiceBus").Bind(option);
                return option;
            });
            services.AddScoped<IDomainEventDispatcher, OutProcDomainEventDispatcher>();
            return services;
        }

        public static IServiceCollection AddNoOpServiceBus(this IServiceCollection services)
        {
            services.AddSingleton<IDomainEventDispatcher, InProcDomainEventDispatcher>();
            return services;
        }
    }
}