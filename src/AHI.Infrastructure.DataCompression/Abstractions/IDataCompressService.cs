using System.Collections.Generic;
using AHI.Infrastructure.DataCompression.Model;

namespace AHI.Infrastructure.DataCompression.Abstraction
{
    public interface IDataCompressService
    {
        IEnumerable<CompressedMetric> Compress(string timestamp, IEnumerable<MetricConfig> metricConfigs, IDictionary<string, object> rawMetrics);
    }
}
