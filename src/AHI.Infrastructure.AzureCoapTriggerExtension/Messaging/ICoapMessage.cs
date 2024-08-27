namespace AHI.Infrastructure.AzureCoapTriggerExtension.Messaging
{
    public interface ICoapMessage
    {
        /// <summary>
        /// Gets the topic of the message.
        /// </summary>
        string Topic { get; }

        /// <summary>
        /// Gets the messages as an array of <see cref="byte"/>.
        /// </summary>
        /// <returns>The message as an array of <see cref="byte"/>.</returns>
        byte[] GetMessage();
    }
}
