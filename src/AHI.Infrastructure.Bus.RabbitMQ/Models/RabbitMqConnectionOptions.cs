namespace AHI.Infrastructure.Bus.ServiceBus.Model
{
    /// <summary>
    /// An options model that "contains" sections for producing and consuming connections of a RabbitMQ clients
    /// <see cref="IQueueService"/>, <see cref="IConsumingService"/> and <see cref="IProducingService"/>.
    /// </summary>
    public class RabbitMqConnectionOptions
    {
        /// <summary>
        /// Producer connection.
        /// </summary>
        public RabbitMqClientOptions ProducerOptions { get; set; }
    }
}