using Microsoft.Azure.WebJobs;
using System;

namespace AHI.Infrastructure.AzureCoapTriggerExtension.Config
{
    public static class CoapWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddCoap(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<CoapExtensionConfigProvider>();

            return builder;
        }
    }
}
