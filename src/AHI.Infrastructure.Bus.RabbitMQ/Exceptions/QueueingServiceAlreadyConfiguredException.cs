using System;

namespace AHI.Infrastructure.Bus.ServiceBus.Exception
{
    /// <summary>
    /// An exception that is thrown when queuing service of the same type configured twice.
    /// </summary>
    public class QueueingServiceAlreadyConfiguredException : System.Exception
    {
        /// <summary>
        /// Type of queuing service.
        /// </summary>
        public Type QueueingServiceType { get; }

        public QueueingServiceAlreadyConfiguredException(Type type, string message) : base(message)
        {
            QueueingServiceType = type;
        }
    }
}