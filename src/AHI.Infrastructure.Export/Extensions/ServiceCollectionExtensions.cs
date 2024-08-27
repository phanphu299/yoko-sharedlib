using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Export.Builder;
using AHI.Infrastructure.Export.Abstraction;
using AHI.Infrastructure.Export.ErrorTracking;

namespace AHI.Infrastructure.Export.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExportingServices(this IServiceCollection services)
        { 
            services.AddScoped<IExportTrackingService, ExportTrackingService>();
            services.AddScoped<ExcelExportBuider>();
            services.AddScoped(typeof(JsonExportBuilder<>));
            return services;
        }
    }
}