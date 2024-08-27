using System;
using Microsoft.Extensions.Caching.Memory;
using AHI.Infrastructure.DataCompression.Abstraction;
using AHI.Infrastructure.DataCompression.Constant;
using AHI.Infrastructure.DataCompression.Model;

namespace AHI.Infrastructure.DataCompression.Internal
{
    internal class SwingDoorCompressor : IDataCompressor
    {
        private readonly IMemoryCache _memoryCache;
        public SwingDoorCompressor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public CompressedMetric Compress(MetricConfig metricConfig, object rawValue, string timestamp)
        {
            var metricRawValue = rawValue.ToString();
            if (!metricConfig.EnableSwingDoor)
            {
                return new CompressedMetric(metricConfig.MetricName, metricRawValue, timestamp);
            }

            var metricName = metricConfig.MetricName;
            var key = $"{Constants.MetricRefKey}_{metricName}";
            var refData = _memoryCache.Get<ReferenceMetric>(key);

            var newPoint = double.Parse(metricRawValue);
            var refPoint = refData.RefPoint;
            var refTimestamp = refData.RefTimestamp;
            var heldPoint = refData.HeldPoint;
            var heldTimeStamp = refData.HeldTimeStamp;

            if (string.IsNullOrEmpty(refPoint))
            {
                refData.RefPoint = metricRawValue;
                refData.RefTimestamp = timestamp;

                _memoryCache.Set(key, refData);
                return new CompressedMetric(metricName, metricRawValue, timestamp);
            }

            if (newPoint.Equals(0))
            {
                refData.RefPoint = metricRawValue;
                refData.RefTimestamp = timestamp;
                refData.HeldPoint = string.Empty;
                refData.HeldTimeStamp = timestamp;
                refData.MinAngle = 0;
                refData.MaxAngle = 0;

                _memoryCache.Set(key, refData);
                return new CompressedMetric(metricName, metricRawValue, timestamp);
            }

            var newHeldPoint = CalculateNewHeldPoint(metricConfig, refPoint, refTimestamp, metricRawValue, timestamp);
            if (string.IsNullOrEmpty(heldPoint))
            {
                refData.HeldPoint = metricRawValue;
                refData.HeldTimeStamp = timestamp;
                refData.MinAngle = newHeldPoint.MinAngle;
                refData.MaxAngle = newHeldPoint.MaxAngle;

                _memoryCache.Set(key, refData);
                return null;
            }

            if (refData.MinAngle <= newHeldPoint.Angle && newHeldPoint.Angle <= refData.MaxAngle)
            {
                //drop old held point, new point become new held point
                refData.HeldPoint = metricRawValue;
                refData.HeldTimeStamp = timestamp;
                refData.MinAngle = Math.Max(refData.MinAngle, newHeldPoint.MinAngle);
                refData.MaxAngle = Math.Min(refData.MaxAngle, newHeldPoint.MaxAngle);

                _memoryCache.Set(key, refData);
                return null;
            }
            else
            {
                //held point become ref point
                var nHeldPoint = CalculateNewHeldPoint(metricConfig, heldPoint, heldTimeStamp, metricRawValue, timestamp);

                refData.RefPoint = heldPoint;
                refData.RefTimestamp = heldTimeStamp;
                refData.HeldPoint = metricRawValue;
                refData.HeldTimeStamp = timestamp;
                refData.MinAngle = nHeldPoint.MinAngle;
                refData.MaxAngle = nHeldPoint.MaxAngle;

                _memoryCache.Set(key, refData);
                return new CompressedMetric(metricName, heldPoint, heldTimeStamp);
            }
        }

        private HeldPoint CalculateNewHeldPoint(MetricConfig metricConfig, string referencePoint, string refTimestamp, string metricRawValue, string newTimestamp)
        {
            var compDevPlus = metricConfig.CompDevPlus;
            var compDevMinus = metricConfig.CompDevMinus;
            if (compDevMinus < 0)
                compDevMinus *= -1;

            var refPoint = double.Parse(referencePoint);
            var newPoint = double.Parse(metricRawValue);
            var maxDev = newPoint + compDevPlus;
            var minDev = newPoint - compDevMinus;

            var refDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(refTimestamp) / 1000).UtcDateTime;
            var newDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(newTimestamp) / 1000).UtcDateTime;
            var timestampDiff = newDate.Subtract(refDate).TotalSeconds;

            var firstAngle = ((180 / Math.PI) * Math.Atan(Math.Abs(maxDev - refPoint) / timestampDiff));
            var secondAngle = ((180 / Math.PI) * Math.Atan(Math.Abs(minDev - refPoint) / timestampDiff));
            var newPointAngle = ((180 / Math.PI) * Math.Atan(Math.Abs(newPoint - refPoint) / timestampDiff));

            var min = Math.Min(firstAngle, secondAngle);
            var max = Math.Max(firstAngle, secondAngle);
            return new HeldPoint(Math.Round(newPointAngle, 2), Math.Round(min, 2), Math.Round(max, 2));
        }
    }
}
