using AHI.Infrastructure.Import.Abstraction;
using AHI.Infrastructure.Import.BaseExcelParser;
using AHI.Infrastructure.Import.ErrorTracking;
using Microsoft.Extensions.DependencyInjection;

namespace AHI.Infrastructure.Import.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddImportingServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IParserContext, ParserContext>();
            serviceCollection.AddScoped(typeof(IImportTrackingService<>), typeof(ImportTrackingService<>));
            // serviceCollection.AddScoped<IDictionary<Type, IValidator>>(service =>
            // {
            //     return new Dictionary<Type, IValidator>
            //     {
            //     };
            // });
        }
    }
}
