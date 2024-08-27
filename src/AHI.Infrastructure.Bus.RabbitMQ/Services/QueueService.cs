using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using AHI.Infrastructure.Bus.ServiceBus.Model;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.Bus.ServiceBus.Exception;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace AHI.Infrastructure.Bus.ServiceBus.Service
{
    /// <summary>
    /// Implementation of the custom RabbitMQ queue service.
    /// </summary>
    public sealed class QueueService : IProducingService, IDisposable
    {
        public IConnection Connection { get; private set; }
        public IModel Channel { get; private set; }

        private readonly ILogger<QueueService> _logger;
        private readonly IRabbitMqConnectionFactory _rabbitMqConnectionFactory;

        private readonly object _lock = new object();

        public QueueService(
            Guid containerId,
            ILogger<QueueService> logger,
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<RabbitMqConnectionOptionsContainer> connectionOptionsContainers)
        {
            var optionsContainer = connectionOptionsContainers.FirstOrDefault(x => x.ContainerId == containerId);
            if (optionsContainer is null)
            {
                throw new ArgumentException($"Connection options container for {nameof(QueueService)} with the guid {containerId} is not found.", nameof(connectionOptionsContainers));
            }

            _logger = logger;
            _rabbitMqConnectionFactory = rabbitMqConnectionFactory;

            ConfigureConnectionInfrastructure(optionsContainer);
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.CallbackException -= HandleConnectionCallbackException;
                if (Connection is IAutorecoveringConnection connection)
                {
                    connection.ConnectionRecoveryError -= HandleConnectionRecoveryError;
                }
            }

            if (Channel != null)
            {
                Channel.CallbackException -= HandleChannelCallbackException;
                Channel.BasicRecoverOk -= HandleChannelBasicRecoverOk;
            }


            if (Channel?.IsOpen == true)
            {
                Channel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (Connection?.IsOpen == true)
            {
                Connection.Close();
            }

            Channel?.Dispose();
            Connection?.Dispose();
        }

        public void Send<T>(T @object, string exchangeName, string routingKey) where T : class
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var bytes = @object.Serialize();
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        public void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            lock (_lock)
            {
                Channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey = "all") where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey)).ConfigureAwait(false);

        private IBasicProperties CreateJsonProperties()
        {
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            return properties;
        }

        private void HandleConnectionCallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        private void HandleConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        private void HandleChannelBasicRecoverOk(object sender, EventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogInformation("Connection has been reestablished.");
        }

        private void HandleChannelCallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
        }

        private void ConfigureConnectionInfrastructure(RabbitMqConnectionOptionsContainer optionsContainer)
        {
            Connection = _rabbitMqConnectionFactory.CreateRabbitMqConnection(optionsContainer?.Options?.ProducerOptions);
            if (Connection != null)
            {
                Connection.CallbackException += HandleConnectionCallbackException;
                if (Connection is IAutorecoveringConnection connection)
                {
                    connection.ConnectionRecoveryError += HandleConnectionRecoveryError;
                }
                Channel = Connection.CreateModel();
                Channel.CallbackException += HandleChannelCallbackException;
                Channel.BasicRecoverOk += HandleChannelBasicRecoverOk;
            }
        }

        private void EnsureProducingChannelIsNotNull()
        {
            if (Channel is null)
            {
                throw new ProducingChannelIsNullException($"Producing channel is null. Configure {nameof(IProducingService)}.");
            }
        }

        private void ValidateArguments(string exchangeName, string routingKey)
        {
            if (string.IsNullOrEmpty(routingKey))
            {
                throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));
            }
        }
    }
}