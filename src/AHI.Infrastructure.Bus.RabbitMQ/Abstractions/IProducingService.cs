using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace AHI.Infrastructure.Bus.ServiceBus.Abstraction
{
    /// <summary>
    /// Custom RabbitMQ producing service interface.
    /// </summary>
    public interface IProducingService
    {
        /// <summary>
        /// RabbitMQ producing connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// RabbitMQ producing channel.
        /// </summary>
        IModel Channel { get; }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class;
    }
}