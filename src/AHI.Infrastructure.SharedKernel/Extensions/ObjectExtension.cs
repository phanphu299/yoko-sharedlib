using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace AHI.Infrastructure.SharedKernel.Extension
{
    public static class ObjectExtension
    {
        public static bool TryParseJArray<T>(this object value, out T val)
        {
            var success = false;
            T outValue = default;
            try
            {
                if (value != null)
                {
                    var jArray = (JArray)value;
                    outValue = jArray.ToObject<T>();
                    success = true;
                }
            }
            catch { }
            val = outValue;
            return success;
        }
        public static DateTime ValidateDateTime(this object value, string datetimeFormat)
        {
            var valid = DateTime.TryParseExact(value.ToString(), datetimeFormat, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var d);
            if (!valid)
            {
                throw new FormatException();
            }
            return d;
        }
        public static bool ValidateBoolean(this object value)
        {
            var valid = bool.TryParse(value.ToString(), out var b);
            if (!valid)
            {
                throw new FormatException();
            }
            return b;
        }

        public static int ValidateInteger(this object value)
        {
            if (value is int || value is short || value is byte)
                return (int)value;
            if (value is long)
            {
                if ((long)value < int.MinValue || (long)value > int.MaxValue)
                    throw new OverflowException();
                else
                    return (int)value;
            }
            if (value is double)
            {
                if ((double)value < int.MinValue || (double)value > int.MaxValue)
                    throw new OverflowException();
                else
                    return Convert.ToInt32(value);
            }
            var valid = double.TryParse(value.ToString(), out var i);
            if (!valid)
            {
                throw new FormatException();
            }
            return (int)i;
        }
        public static long ValidateLong(this object value)
        {
            if (value is int || value is short || value is byte || value is long)
                return Convert.ToInt64(value);

            if (value is double)
            {
                if ((double)value < long.MinValue || (double)value > long.MaxValue)
                    throw new OverflowException();
                else
                    return Convert.ToInt64(value);
            }
            var valid = long.TryParse(value.ToString(), out var i);
            if (!valid)
            {
                throw new FormatException();
            }
            return (long)i;
        }

        public static double ValidateDouble(this object value)
        {
            if (value is long || value is int || value is short || value is byte || value is float || value is double)
                return Convert.ToDouble(value);
            var valid = double.TryParse(value.ToString(), out var d);
            if (!valid)
            {
                throw new FormatException();
            }
            return d;
        }
        public static float ValidateFloat(this object value)
        {
            if (value is long || value is int || value is short || value is byte)
                return (float)Convert.ToInt64(value);
            if (value is float)
                return (float)value;
            if (value is double)
            {
                if ((double)value < float.MinValue || (double)value > float.MaxValue)
                    throw new OverflowException();
            }
            var valid = float.TryParse(value.ToString(), out var d);
            if (!valid)
            {
                throw new FormatException();
            }
            return d;
        }
        // public static decimal ValidateDecimal(this object value)
        // {
        //     if (value is long || value is int || value is short || value is byte)
        //         return new decimal((long)value);
        //     if (value is decimal)
        //     {
        //         return (decimal)value;
        //     }
        //     return decimal.Parse(value.ToString());
        // }
        public static DateTime ValidateTimestamp(this object value)
        {
            var timestamp = value.ValidateLong();
            try
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                return dateTimeOffset.DateTime;
            }
            catch (ArgumentOutOfRangeException)
            {
                try
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                    return dateTimeOffset.DateTime;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new OverflowException();
                }
            }
        }
        public static string ValidateText(this object value, int maxLength)
        {
            var v = value.ToString();
            if (v.Length > maxLength)
            {
                throw new OverflowException();
            }
            return v;
        }
    }
}