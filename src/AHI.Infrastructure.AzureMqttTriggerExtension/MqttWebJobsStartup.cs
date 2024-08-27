using AHI.Infrastructure.AzureMqttTriggerExtension;
using AHI.Infrastructure.AzureMqttTriggerExtension.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(MqttWebJobsStartup))]
namespace AHI.Infrastructure.AzureMqttTriggerExtension
{
    public class MqttWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddMqtt();
        }
    }
}
