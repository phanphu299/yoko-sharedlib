namespace AHI.Infrastructure.Bus.Kafka.Model
{
    public class KafkaOption
    {
        public bool? Enabled { get; set; }
        public string BootstrapServers { get; set; }
        public KafkaProducerOption Producer { get; set; }
        public KafkaConsumerOption Consumer { get; set; }
    }
}