using System;
using System.Security.Cryptography;
using System.Text;

namespace AHI.Infrastructure.SharedKernel.Extension
{
    public static class StringExtensions
    {
        public static string RemoveFileToken(this string fileName)
        {
            var index = fileName?.IndexOf("?token=") ?? -1;
            return index < 0 ? fileName ?? string.Empty : fileName.Remove(index);
        }

        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else
                return input;
        }

        public static string CalculateMd5Hash(this string input)
        {
            using (var md5Hash = MD5.Create())
            {
                // Byte array representation of source string
                var sourceBytes = Encoding.UTF8.GetBytes(input);

                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                // Convert hash byte array to string
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }

        public static DateTime UnixTimeStampToDateTime(this string unixTimeStamp)
        {
            if (long.TryParse(unixTimeStamp, out var unixTimeStampValue))
            {
                try
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStampValue);
                    return dateTimeOffset.DateTime;
                }
                catch
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStampValue);
                    return dateTimeOffset.DateTime;
                }
            }
            else if (DateTimeOffset.TryParse(unixTimeStamp, out var dateTimeWithOffset))
            {
                // fallback to datetime in utc
                return dateTimeWithOffset.DateTime;
            }
            else if (DateTime.TryParseExact(unixTimeStamp, "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var dateWithoutOffset))
            {
            }
            else if (DateTime.TryParse(unixTimeStamp, out var dateTime))
            {
                return dateTime;
            }
            throw new Exception($"Can not parse DateTime for string - {unixTimeStamp}");
        }
    }
}
