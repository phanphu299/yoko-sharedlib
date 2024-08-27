using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.SystemContext.Abstraction;
using AHI.Infrastructure.SharedKernel;

namespace AHI.Infrastructure.SystemContext.Extension
{
    public static class ServiceExtension
    {
        public static void AddSystemContextService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLoggingService();
            serviceCollection.AddScoped<ISystemContext, Internal.SystemContext>();
        }
    }
}