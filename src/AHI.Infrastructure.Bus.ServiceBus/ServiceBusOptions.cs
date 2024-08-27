namespace AHI.Infrastructure.Bus.ServiceBus.Option
{
    public class ServiceBusOptions
    {
        public string Host { get; set; }
        public string KeyName { get; set; }
        public string SharedAccessKey { get; set; }
        public string ConnectionString => $"Endpoint=sb://{Host}.servicebus.windows.net/;SharedAccessKeyName={KeyName};SharedAccessKey={SharedAccessKey}";
    }
}
