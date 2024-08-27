namespace AHI.Infrastructure.DataCompression.Model
{
    public class MetricConfig
    {
        public string MetricName { get; set; }
        public bool EnableDeadBand { get; set; }
        public bool EnableSwingDoor { get; set; }
        public int IdleTimeout { get; set; }
        public double ExDevPlus { get; set; }
        public double ExDevMinus { get; set; }
        public double CompDevPlus { get; set; }
        public double CompDevMinus { get; set; }

        public void Deconstruct(out string metricName, out int idleTimeout)
        {
            metricName = MetricName;
            idleTimeout = IdleTimeout;
        }

        public void Deconstruct(out string metricName, out double exDevPlus, out double exDevMinus)
        {
            metricName = MetricName;
            exDevPlus = ExDevPlus;
            exDevMinus = ExDevMinus;
        }
    }
}
