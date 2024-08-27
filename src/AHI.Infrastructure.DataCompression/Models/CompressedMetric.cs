namespace AHI.Infrastructure.DataCompression.Model
{
    public class CompressedMetric
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Timestamp { get; set; }

        public CompressedMetric(string metricName, object value, string timestamp)
        {
            Name = metricName;
            Value = value;
            Timestamp = timestamp;
        }
    }
}
