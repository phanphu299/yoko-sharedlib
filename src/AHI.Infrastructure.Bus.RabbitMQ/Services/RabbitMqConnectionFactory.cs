using System;
using System.Threading;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Exception;
using AHI.Infrastructure.Bus.ServiceBus.Model;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;

namespace AHI.Infrastructure.Bus.ServiceBus.Service
{
    /// <summary>
    /// Service that is responsible for creating RabbitMQ connections depending on options <see cref="RabbitMqClientOptions"/>.
    /// </summary>
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        /// <summary>
        /// Create a RabbitMQ connection.
        /// </summary>
        /// <param name="options">An instance of options <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>An instance of connection <see cref="IConnection"/>.</returns>
        /// <remarks>If options parameter is null the method return null too.</remarks>
        public IConnection CreateRabbitMqConnection(RabbitMqClientOptions options)
        {
            if (options is null)
                return null;

            var factory = new ConnectionFactory
            {
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = options.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = options.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                HandshakeContinuationTimeout = options.HandshakeContinuationTimeout,
                RequestedChannelMax = options.RequestedChannelMax,
                RequestedFrameMax = options.RequestedFrameMax,
                MaxMessageSize = options.MaxMessageSize,
                RequestedHeartbeat = options.RequestedHeartbeat,
                DispatchConsumersAsync = true
            };

            return string.IsNullOrEmpty(options.ClientProvidedName)
                    ? CreateConnection(options, factory)
                    : CreateNamedConnection(options, factory);
        }

        private static IConnection CreateConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            factory.HostName = options.HostName;
            return TryToCreateConnection(factory.CreateConnection, options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
        }

        private static IConnection CreateNamedConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            factory.HostName = options.HostName;
            return TryToCreateConnection(() => factory.CreateConnection(options.ClientProvidedName), options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
        }

        private static IConnection TryToCreateConnection(Func<IConnection> connectionFunction, int numberOfRetries, int timeoutMilliseconds)
        {
            ValidateArguments(numberOfRetries, timeoutMilliseconds);

            var attempts = 0;
            BrokerUnreachableException latestException = null;

            while (attempts < numberOfRetries)
            {
                try
                {
                    if (attempts > 0)
                    {
                        Thread.Sleep(timeoutMilliseconds);
                    }

                    return connectionFunction();
                }
                catch (BrokerUnreachableException exception)
                {
                    attempts++;
                    latestException = exception;
                }
            }

            throw new InitialConnectionException($"Could not establish an initial connection in {numberOfRetries} retries", latestException)
            {
                NumberOfRetries = attempts
            };
        }

        private static void ValidateArguments(int numberOfRetries, int timeoutMilliseconds)
        {
            if (numberOfRetries < 1)
            {
                throw new ArgumentException("Number of retries should be a positive number.", nameof(numberOfRetries));
            }

            if (timeoutMilliseconds < 1)
            {
                throw new ArgumentException("Initial reconnection timeout should be a positive number.", nameof(timeoutMilliseconds));
            }
        }
    }
}