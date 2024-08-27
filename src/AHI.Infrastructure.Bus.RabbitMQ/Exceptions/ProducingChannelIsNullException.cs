namespace AHI.Infrastructure.Bus.ServiceBus.Exception
{
    /// <summary>
    /// An exception that is thrown during the publication of a message when the channel is null.
    /// </summary>
    public class ProducingChannelIsNullException : System.Exception
    {
        public ProducingChannelIsNullException(string message) : base(message)
        {
        }
    }
}