using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Cache.Abstraction;
using Microsoft.Extensions.Configuration;
using AHI.Infrastructure.SharedKernel;

namespace AHI.Infrastructure.Cache.Redis.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNoOpCache(this IServiceCollection services)
        {
            services.AddScoped<ICache, NoOpCache>();
            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services)
        {
            services.AddSingleton<CacheOptions>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                var cacheOptions = new CacheOptions();
                configuration.GetSection("Redis").Bind(cacheOptions);
                return cacheOptions;
            });
            services.AddSingleton<ICache, RedisCache>();
            services.AddLoggingService();
            return services;
        }
    }
}
