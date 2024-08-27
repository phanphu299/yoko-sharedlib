using System.Threading.Tasks;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using RabbitMQ.Client;
using AHI.Infrastructure.Bus.ServiceBus.Option;
using Polly;
using RabbitMQ.Client.Exceptions;
using System;
using AHI.Infrastructure.SharedKernel.Extension;
using System.Collections.Generic;
using AHI.Infrastructure.Bus.ServiceBus.Helpers;

namespace AHI.Infrastructure.Bus.ServiceBus.Internal
{
    internal class OutProcDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly RabbitMQOptions _option;

        public OutProcDomainEventDispatcher(RabbitMQOptions option)
        {
            _option = option;
        }

        public Task SendAsync<T>(T message, string routingKey = "all") where T : BusEvent
        {
            var policy = PolicyHelper.CreatePolicy();

            var factory = new ConnectionFactory()
            {
                Uri = new System.Uri(_option.ConnectionString),
                HandshakeContinuationTimeout = TimeSpan.FromSeconds(_option.HandshakeContinuationTimeout),
                RequestedChannelMax = _option.RequestedChannelMax,
                RequestedFrameMax = _option.RequestedFrameMax,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(_option.RequestedConnectionTimeout),
                MaxMessageSize = _option.MaxMessageSize,
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Send(channel, policy, message, routingKey);
            }
            return Task.CompletedTask;
        }

        public Task SendAsync<T>(List<T> messages, string routingKey = "all") where T : BusEvent
        {
            var policy = PolicyHelper.CreatePolicy();

            var factory = new ConnectionFactory() { Uri = new System.Uri(_option.ConnectionString), HandshakeContinuationTimeout = TimeSpan.FromSeconds(50) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                foreach (var message in messages)
                {
                    Send(channel, policy, message, routingKey);
                }
            }
            return Task.CompletedTask;
        }

        private void Send<T>(IModel channel, Policy policy, T message, string routingKey) where  T : BusEvent
        {
            var msg = new { Message = message }; // need to wrap into message payload
            var data = msg.Serialize();
            policy.Execute(() =>
            {
                channel.BasicPublish(exchange: message.TopicName,
                                        routingKey: routingKey,
                                        body: data);
            });
        }
    }
}