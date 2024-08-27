using AHI.Infrastructure.DataCompression.Model;

namespace AHI.Infrastructure.DataCompression.Abstraction
{
    public interface IDataCompressor
    {
        CompressedMetric Compress(MetricConfig metricConfig, object metricRawValue, string timestamp);
    }
}
