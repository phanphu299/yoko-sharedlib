using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.SharedKernel;
using AHI.Infrastructure.Cache.Redis.Extension;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.UserContext.Service.Abstraction;
using AHI.Infrastructure.UserContext.Internal;
using AHI.Infrastructure.UserContext.Service;

namespace AHI.Infrastructure.UserContext.Extension
{
    public static class ServiceExtension
    {
        public static void AddUserContextService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLoggingService();
            serviceCollection.AddRedisCache();
            serviceCollection.AddMultiTenantService();
            serviceCollection.AddScoped<IUserContext, Internal.UserContext>();
            serviceCollection.AddScoped<ISecurityService, SecurityService>();
            serviceCollection.AddScoped<ISecurityContext, SecurityContext>();
            serviceCollection.AddHttpClient("user-function", (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                var userApi = configuration["Function:User"];
                if (string.IsNullOrEmpty(userApi))
                {
                    throw new System.Exception($"Function__User is not configured in appsettings.");
                }
                client.BaseAddress = new Uri(userApi);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();
        }
    }
}