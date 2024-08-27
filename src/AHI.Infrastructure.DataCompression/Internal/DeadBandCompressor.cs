using Microsoft.Extensions.Caching.Memory;
using AHI.Infrastructure.DataCompression.Abstraction;
using AHI.Infrastructure.DataCompression.Constant;
using AHI.Infrastructure.DataCompression.Model;

namespace AHI.Infrastructure.DataCompression.Internal
{
    internal class DeadBandCompressor : IDataCompressor
    {
        private readonly IDataCompressor _next;
        private readonly IMemoryCache _memoryCache;
        public DeadBandCompressor(IMemoryCache memoryCache, IDataCompressor next)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public CompressedMetric Compress(MetricConfig metricConfig, object rawValue, string timestamp)
        {
            var metricRawValue = rawValue.ToString();
            if (!metricConfig.EnableDeadBand)
            {
                return _next == null ? new CompressedMetric(metricConfig.MetricName, metricRawValue, timestamp)
                            : _next.Compress(metricConfig, metricRawValue, timestamp);
            }

            var (metricName, exDevPlus, exDevMinus) = metricConfig;
            var key = $"{Constants.MetricRefKey}_{metricName}";
            if (exDevMinus < 0)
                exDevMinus *= -1;

            var newPoint = double.Parse(metricRawValue);
            var refData = _memoryCache.Get<ReferenceMetric>(key);
            var refPoint = refData.RefPoint;
            var maxDev = string.IsNullOrEmpty(refPoint) ? float.MinValue : double.Parse(refPoint) + exDevPlus;
            var minDev = string.IsNullOrEmpty(refPoint) ? float.MaxValue : double.Parse(refPoint) - exDevMinus;

            if (refPoint != null && !newPoint.Equals(0) && minDev <= newPoint && newPoint <= maxDev)
            {
                return null;
            }
            else
            {
                if (_next != null && metricConfig.EnableSwingDoor)
                {
                    return _next.Compress(metricConfig, metricRawValue, timestamp);
                }

                refData.RefPoint = metricRawValue;
                refData.RefTimestamp = timestamp;
                _memoryCache.Set(key, refData);
                return new CompressedMetric(metricName, metricRawValue, timestamp);
            }
        }
    }
}
