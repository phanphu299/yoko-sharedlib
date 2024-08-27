using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.DataCompression.Abstraction;
using AHI.Infrastructure.DataCompression.Model;
using AHI.Infrastructure.DataCompression.Constant;
using Microsoft.Extensions.Caching.Memory;


namespace AHI.Infrastructure.DataCompression.Internal
{
    internal class DataCompressService : IDataCompressService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDataCompressor _dataCompressor;

        public DataCompressService(IMemoryCache memoryCache, IDataCompressor compressor)
        {
            _memoryCache = memoryCache;
            _dataCompressor = compressor;
        }
        public IEnumerable<CompressedMetric> Compress(string timestamp, IEnumerable<MetricConfig> metricConfigs, IDictionary<string, object> rawMetrics)
        {
            if (metricConfigs == null || !metricConfigs.Any())
                return rawMetrics.Select(raw => new CompressedMetric(raw.Key, raw.Value, timestamp));

            var results = new List<CompressedMetric>();
            var obj = new Object();
            Parallel.ForEach(rawMetrics, (raw) =>
            {
                var mConfig = metricConfigs.FirstOrDefault(cfg => cfg.MetricName.Equals(raw.Key) && (cfg.EnableDeadBand || cfg.EnableSwingDoor));
                if (mConfig == null)
                {
                    lock (obj)
                    {
                        results.Add(new CompressedMetric(raw.Key, raw.Value, timestamp));
                    }
                }
                else
                {
                    CheckInterruptedData(mConfig, timestamp);
                    var compressed = _dataCompressor.Compress(mConfig, raw.Value, timestamp);
                    if (compressed != null)
                    {
                        lock (obj)
                        {
                            results.Add(compressed);
                        }
                    }
                }
            });

            return results;
        }

        private void CheckInterruptedData(MetricConfig metricConfig, string newTimestamp)
        {
            var (metricName, idleTimeout) = metricConfig;
            var key = $"{Constants.MetricRefKey}_{metricName}";

            var refData = _memoryCache.Get<ReferenceMetric>(key);
            if (refData == null)
            {
                refData = new ReferenceMetric();
                refData.AutoTimeout = 5; //set default to 5 seconds
                _memoryCache.Set(key, refData);
                return;
            }

            if (string.IsNullOrEmpty(refData.HeldTimeStamp))
                return;

            var refDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(refData.HeldTimeStamp) / 1000).UtcDateTime;
            var newDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(newTimestamp) / 1000).UtcDateTime;
            var msgIdleInSeconds = newDate.Subtract(refDate).TotalSeconds;
            var userSetTimeout = idleTimeout * 60; //convert minutes to seconds

            //if msg idle timeout > IdleTimeout set by user, reset compressor
            //incase IdleTimeout is not set by user, reset compressor after 3 times of autoTimeout
            if ((userSetTimeout > 0 && msgIdleInSeconds > userSetTimeout) || msgIdleInSeconds > (refData.AutoTimeout * 3))
            {
                var emptyRef = new ReferenceMetric();
                emptyRef.AutoTimeout = 5; //set default to 5 seconds
                _memoryCache.Set(key, emptyRef);
            }
            else
            {
                var newAutoTimeout = Math.Round((refData.AutoTimeout + msgIdleInSeconds) / 2);
                refData.AutoTimeout = newAutoTimeout;
                _memoryCache.Set(key, refData);
            }
        }
    }
}