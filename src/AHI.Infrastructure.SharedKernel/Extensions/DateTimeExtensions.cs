using System;

namespace AHI.Infrastructure.SharedKernel
{
    public static class DateTimeExtensions
    {
        public const string TIMESTAMP_FORMAT = "yyyyMMddHHmmss";

        /// <summary>
        /// by default datetime type will have nanosecond, we cut off its nanosecond (yyyy-MM-dd HH:mm:ss.ffffff)
        /// ex: 
        ///     2022-07-06T13:22:09.696041 (has nanosecond)
        ///     2022-07-06T13:22:09.696000 (nanosecond is cutted off)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime CutOffNanoseconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
        }

        public static DateTimeOffset ToUtcDateTimeOffset(this DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => new DateTimeOffset(dateTime),
                DateTimeKind.Local => new DateTimeOffset(dateTime.ToUniversalTime()),
                _ => new DateTimeOffset(dateTime, TimeSpan.Zero)
            };
        }

        public static string ToTimestamp(this DateTime datetime, string offset = null)
        {
            var utc = datetime.ToUtcDateTimeOffset();
            if (!string.IsNullOrEmpty(offset))
                utc = utc.ToOffset(TimeSpan.Parse(offset));
            return utc.ToString(TIMESTAMP_FORMAT);
        }

        public static string ToValidOffset(this string offset)
        {
            var builder = new System.Text.StringBuilder(offset);
            if (offset.StartsWith('+'))
                builder.Remove(0, 1);
            if (!offset.Contains(':'))
                builder.Append(":00");
            return builder.ToString();
        }
    }
}