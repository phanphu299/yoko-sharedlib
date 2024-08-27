namespace AHI.Infrastructure.DataCompression.Model
{
    internal class ReferenceMetric
    {
        public double AutoTimeout { get; set; }
        public string RefPoint { get; set; }
        public string RefTimestamp { get; set; }
        public string HeldPoint { get; set; }
        public string HeldTimeStamp { get; set; }
        public double MinAngle { get; set; }
        public double MaxAngle { get; set; }
    }
}
