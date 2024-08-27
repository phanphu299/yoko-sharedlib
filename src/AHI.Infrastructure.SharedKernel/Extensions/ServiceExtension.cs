using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.SharedKernel.Abstraction;

namespace AHI.Infrastructure.SharedKernel
{
    public static class ServiceExtension
    {
        public static void AddLoggingService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
        }
    }
}