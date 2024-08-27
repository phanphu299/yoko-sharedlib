using System;
using System.Threading.Tasks;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using RabbitMQ.Client.Exceptions;
using Polly;
using System.Collections.Generic;
using AHI.Infrastructure.Bus.ServiceBus.Helpers;

namespace AHI.Infrastructure.Bus.ServiceBus.Internal
{
    internal class RabbitMQOutProcEventDispatcher : IDomainEventDispatcher
    {
        private readonly IProducingService _producingService;

        public RabbitMQOutProcEventDispatcher(IProducingService producingService)
        {
            _producingService = producingService;
        }

        public Task SendAsync<T>(T message, string routingKey = "all") where T : BusEvent
        {
            var policy = PolicyHelper.CreatePolicy();
            SendAsync(policy, message, routingKey);
            return Task.CompletedTask;
        }

        public Task SendAsync<T>(List<T> messages, string routingKey = "all") where T : BusEvent
        {
            var policy = PolicyHelper.CreatePolicy();

            foreach (var message in messages)
            {
                SendAsync(policy, message, routingKey);
            }

            return Task.CompletedTask;
        }

        private Task SendAsync<T>(Policy policy, T message, string routingKey) where T : BusEvent
        {
            // Need to wrap into message payload
            var wrappedMessage = new { Message = message };
            policy.Execute(() =>
            {
                _producingService.SendAsync(wrappedMessage, message.TopicName, routingKey);
            });
            return Task.CompletedTask;
        }
    }
}