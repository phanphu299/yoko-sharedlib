using AHI.Infrastructure.AzureCoapTriggerExtension.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AHI.Infrastructure.AzureCoapTriggerExtension.Bindings
{
    internal class CoapTriggerBindingProvider : ITriggerBindingProvider
    {
        private readonly INameResolver _nameResolver;

        public CoapTriggerBindingProvider(INameResolver nameResolver)
        {
            _nameResolver = nameResolver;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<CoapTriggerAttribute>(false);

            if (attribute == null)
                return Task.FromResult<ITriggerBinding>(null);
            if (parameter.ParameterType != typeof(ICoapMessage))
                throw new InvalidOperationException("Invalid parameter type");

            var triggerBinding = new CoapTriggerBinding(parameter, _nameResolver);

            return Task.FromResult<ITriggerBinding>(triggerBinding);
        }
    }
}
