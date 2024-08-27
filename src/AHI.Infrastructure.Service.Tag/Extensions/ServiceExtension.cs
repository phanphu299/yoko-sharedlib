using AHI.Infrastructure.MultiTenancy.Http.Handler;
using AHI.Infrastructure.Service.Tag.Configuration;
using AHI.Infrastructure.Service.Tag.Constant;
using AHI.Infrastructure.Service.Tag.Enum;
using AHI.Infrastructure.Service.Tag.Service;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AHI.Infrastructure.Service.Tag.Extension
{
    public static class ServiceExtension
    {
        public static void AddEntityTagService(this IServiceCollection serviceCollection, DatabaseType dbType)
        {
            DbConfig.DatabaseType = dbType;
            serviceCollection.AddScoped<ITagService, TagService>();
            serviceCollection.AddHttpClient(ServiceTagConstants.HTTP_CLIENT_NAME, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ServiceTagConstants.TAG_ENDPOINT_CONFIGURATION]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();
        }

        public static void AddEntityTagRepository(this IServiceCollection serviceCollection, Type dbContextType)
        {
            DbConfig.DbContextType = dbContextType;
            serviceCollection.AddScoped(typeof(IEntityTagRepository<>), typeof(EntityTagRepository<>));
        }
    }
}