using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Internal;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AHI.Infrastructure.MultiTenancy.Option;

namespace AHI.Infrastructure.MultiTenancy.Extension
{
    public static class ServiceExtension
    {
        public static void AddMultiTenantService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITenantContext, TenantContext>();
            serviceCollection.AddTransient<ClientCrendetialAuthentication>();
            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.TryAddSingleton<MultiTenancyOption>(service =>
            {
                return new MultiTenancyOption()
                {
                    SkipPaths = new[] { "/healthz", "/metrics" }
                };
            });
            serviceCollection.AddHttpClient("identity-service", (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                var identityApi = configuration["Authentication:Authority"];
                if (string.IsNullOrEmpty(identityApi))
                {
                    throw new System.Exception($"Authentication__Authority is not configured in appsettings.");
                }
                client.BaseAddress = new System.Uri(identityApi);
            });
        }
    }
}