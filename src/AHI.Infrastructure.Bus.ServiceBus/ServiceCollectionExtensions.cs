using Azure.Messaging.ServiceBus;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Bus.ServiceBus.Option;
namespace AHI.Infrastructure.Bus.ServiceBus.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services)
        {
            services.AddSingleton<ServiceBusClient>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                var option = new ServiceBusOptions();
                configuration.GetSection("AzureServiceBus").Bind(option);
                return new ServiceBusClient(option.ConnectionString);
            });
            services.AddSingleton<IDomainEventDispatcher, OutProcDomainEventDispatcher>();
            return services;
        }
        public static IServiceCollection AddNoOpServiceBus(this IServiceCollection services)
        {
            services.AddSingleton<IDomainEventDispatcher, InProcDomainEventDispatcher>();
            return services;
        }
    }
}
