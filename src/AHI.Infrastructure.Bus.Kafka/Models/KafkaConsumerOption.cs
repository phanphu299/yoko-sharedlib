namespace AHI.Infrastructure.Bus.Kafka.Model
{
    public class KafkaConsumerOption
    {
        public int? AutoCommitInterval { get; set; }
        public string GroupId { get; set; }
        public int BatchSize { get; set; }
    }
}
