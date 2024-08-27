using System.Diagnostics;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
namespace AHI.Infrastructure.OpenTelemetry
{
    public static class ServiceExtension
    {
        public static void AddOtelTracingService(this IServiceCollection serviceCollection, string serviceName, string serviceVersion)
        {
            serviceCollection.AddSingleton(typeof(ILoggerAdapter<>), typeof(OtelTraceAdapter<>));
            serviceCollection.AddSingleton<ActivitySource>(service => new ActivitySource(serviceName));
            serviceCollection.AddOpenTelemetryTracing(tracerProviderBuilder =>
           {
               tracerProviderBuilder
               //.AddConsoleExporter()
                  .AddOtlpExporter(opt =>
                  {
                      opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                  })
               .AddSource(serviceName)
               .SetResourceBuilder(
                   ResourceBuilder.CreateDefault()
                       .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                .AddHttpClientInstrumentation(option =>
               {
                   option.RecordException = true;
               })
               .AddAspNetCoreInstrumentation(option =>
               {
                   option.RecordException = true;
               })
               .AddSqlClientInstrumentation(option =>
               {
                   option.SetDbStatementForText = true;
                   option.RecordException = true;
               })
               .AddEntityFrameworkCoreInstrumentation(option =>
               {
                   option.SetDbStatementForText = true;
               });
           });
        }
    }
}