namespace AHI.Infrastructure.AzureCoapTriggerExtension.Messaging
{
    public class CoapMessage : ICoapMessage
    {
        private readonly byte[] _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoapMessage"/> class.
        /// </summary>
        public CoapMessage(string topic, byte[] message)
        {
            Topic = topic;
            _message = message;
        }

        /// <summary>
        /// Gets the topic of the message.
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// Gets the messages as an array of <see cref="byte"/>.
        /// </summary>
        /// <returns>The message as an array of <see cref="byte"/>.</returns>
        public byte[] GetMessage()
        {
            return _message;
        }
    }
}
