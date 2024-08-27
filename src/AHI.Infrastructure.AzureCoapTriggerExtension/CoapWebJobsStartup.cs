using AHI.Infrastructure.AzureCoapTriggerExtension;
using AHI.Infrastructure.AzureCoapTriggerExtension.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(CoapWebJobsStartup))]
namespace AHI.Infrastructure.AzureCoapTriggerExtension
{
    public class CoapWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddCoap();
        }
    }
}
