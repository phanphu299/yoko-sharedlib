using AHI.Infrastructure.BlobStorage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AHI.Infrastructure.BlobStorage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlobStorage(this IServiceCollection services)
        {
            services.AddSingleton<BlobStorageOptions>(service =>
            {
                var config = service.GetRequiredService<IConfiguration>();
                var option = new BlobStorageOptions();
                var blobSection = config.GetSection("BlobStorage:Azure");
                blobSection.Bind(option);
                return option;
            });
            services.AddSingleton<IBlobManager, BlobStorageManager>();
            services.AddSingleton<IFileManager, FileStorageManager>();
            return services;
        }
    }
}
