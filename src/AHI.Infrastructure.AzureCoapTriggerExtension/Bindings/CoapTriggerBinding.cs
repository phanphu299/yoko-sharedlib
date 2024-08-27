using AHI.Infrastructure.AzureCoapTriggerExtension.Messaging;
using AzureCoapTriggerExtensions.Listeners;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AHI.Infrastructure.AzureCoapTriggerExtension.Bindings
{
    public class CoapTriggerBinding : ITriggerBinding
    {
        public Type TriggerValueType => typeof(ICoapMessage);
        public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>();
        private readonly ParameterInfo _parameter;
        private readonly INameResolver _nameResolver;

        public CoapTriggerBinding(ParameterInfo parameter, INameResolver nameResolver)
        {
            _nameResolver = nameResolver;
            _parameter = parameter;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            var message = value as ICoapMessage;

            IReadOnlyDictionary<string, object> bindingData = CreateBindingData(message);
            return Task.FromResult<ITriggerData>(new TriggerData(null, bindingData));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            var executor = context.Executor;
            CoapTriggerAttribute attr = _parameter.GetCustomAttribute<CoapTriggerAttribute>();
            attr.TopicName = _nameResolver.ResolveWholeString(attr.TopicName) ?? attr.TopicName;
            var listener = new CoapTriggerListener(executor, attr);

            return Task.FromResult<IListener>(listener);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new TriggerParameterDescriptor
            {
                Name = "Parametr: CoapTrigger",
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "Coap Trigger",
                    Description = "Coap message trigger"
                }
            };
        }

        internal static IReadOnlyDictionary<string, object> CreateBindingData(ICoapMessage value)
        {
            var bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            SafeAddValue(() => bindingData.Add(nameof(value.Topic), value.Topic));
            SafeAddValue(() => bindingData.Add("Message", value.GetMessage()));

            return bindingData;
        }

        private static void SafeAddValue(Action addValue)
        {
            try
            {
                addValue();
            }
            catch
            {
                // some message property getters can throw, based on the
                // state of the message
            }
        }
    }
}
