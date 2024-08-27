using AHI.Infrastructure.Bus.ServiceBus.Model;
using System;

namespace AHI.Infrastructure.Bus.ServiceBus.Option
{
    public class RabbitMQOptions
    {
        public string Host { get; set; }
        public string KeyName { get; set; }
        public string SharedAccessKey { get; set; }
        public int Port { get; set; } = 5672;
        public string ConnectionString => $"amqp://{KeyName}:{SharedAccessKey}@{Host}:{Port}";
        public string ClientProvidedName { get; set; }
        public int HandshakeContinuationTimeout { get; set; } = 50;
        public int RequestedConnectionTimeout { get; set; } = 30;
        public ushort RequestedChannelMax { get; set; } = 2047;
        public uint RequestedFrameMax { get; set; } = 0;
        public uint MaxMessageSize { get; set; } = 0;

        public RabbitMQOptions()
        {
        }

        public RabbitMQOptions(string clientProvidedName)
        {
            ClientProvidedName = clientProvidedName;
        }

        public RabbitMqClientOptions GetRabbitMqClientOptions()
        {
            return new RabbitMqClientOptions()
            {
                HostName = this.Host,
                Port = this.Port,
                UserName = this.KeyName,
                Password = this.SharedAccessKey,
                ClientProvidedName = this.ClientProvidedName,
                HandshakeContinuationTimeout = TimeSpan.FromSeconds(this.HandshakeContinuationTimeout),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(this.RequestedConnectionTimeout),
                RequestedChannelMax = this.RequestedChannelMax,
                RequestedFrameMax = this.RequestedFrameMax,
                MaxMessageSize = this.MaxMessageSize,
            };
        }
    }
}