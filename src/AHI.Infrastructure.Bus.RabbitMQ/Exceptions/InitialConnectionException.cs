namespace AHI.Infrastructure.Bus.ServiceBus.Exception
{
    /// <summary>
    /// An exception that is thrown when an initial connection could not be established even with retry mechanism.
    /// </summary>
    public class InitialConnectionException : System.Exception
    {
        /// <summary>
        /// The number of retries which has been attempted.
        /// </summary>
        public int NumberOfRetries { get; set; }

        public InitialConnectionException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}