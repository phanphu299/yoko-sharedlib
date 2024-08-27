namespace AHI.Infrastructure.Bus.Kafka.Model
{
    public static class KafkaDefaultConfig
    {
        public const int AutoIntervalCommit = 500;
        public const int BatchSizeInBytes = 1000000; // 100 messages with size = 1M = 1000000 bytes;
    }
}
