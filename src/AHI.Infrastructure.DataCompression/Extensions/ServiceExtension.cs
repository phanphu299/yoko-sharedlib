using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.DataCompression.Abstraction;
using AHI.Infrastructure.DataCompression.Internal;

namespace AHI.Infrastructure.DataCompression.Extension
{
    public static class ServiceExtension
    {
        public static void AddDataCompression(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDataCompressor>(service =>
            {
                var cache = service.GetRequiredService<IMemoryCache>();
                var swingdoor = new SwingDoorCompressor(cache);
                var deadband = new DeadBandCompressor(cache, swingdoor);

                return deadband;
            });
            serviceCollection.AddScoped<IDataCompressService, DataCompressService>();
        }
    }
}