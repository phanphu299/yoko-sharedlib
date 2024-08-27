using AHI.Infrastructure.AzureCoapTriggerExtension.Bindings;
using AHI.Infrastructure.AzureCoapTriggerExtension.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;

namespace AHI.Infrastructure.AzureCoapTriggerExtension.Config
{
    public class CoapExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly INameResolver _nameResolver;

        public CoapExtensionConfigProvider(INameResolver nameResolver)
        {
            _nameResolver = nameResolver;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<CoapTriggerAttribute>();
            rule.BindToTrigger<ICoapMessage>(new CoapTriggerBindingProvider(_nameResolver));
        }
    }
}
