using Confluent.Kafka;

namespace AHI.Infrastructure.Bus.Kafka.Model
{
    public class KafkaProducerOption
    {
        public Acks AckMode { get; set; } = Acks.Leader;
        public double? Linger { get; set; } = 100;
        public int BatchSize { get; set; }
    }
}
