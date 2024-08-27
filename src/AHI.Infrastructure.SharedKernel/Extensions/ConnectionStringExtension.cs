using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AHI.Infrastructure.SharedKernel.Extension
{
    public static class ConnectionStringExtension
    {
        public static string BuildConnectionString(this IConfiguration configuration, string targetId)
        {
            var rawConnectionString = configuration["ConnectionStrings:Default"];
            return rawConnectionString.BuildConnectionString(configuration, targetId);
        }
        public static string BuildConnectionString(this string rawConnectionString, IConfiguration configuration, string targetId)
        {
            if (string.IsNullOrEmpty(rawConnectionString))
            {
                throw new ArgumentException(nameof(rawConnectionString));
            }
            var serverEndpoint = configuration["ServerEndpoint"];
            if (rawConnectionString.Contains("{{server}}") && string.IsNullOrEmpty(serverEndpoint))
            {
                throw new ArgumentException("ServerEndpoint is not configure in the appsetting");
            }
            var (id, sequentialNumber) = Extract(targetId);
            if (sequentialNumber > 0)
            {
                serverEndpoint = $"{serverEndpoint}-{sequentialNumber}";
            }
            return rawConnectionString.Replace("{{server}}", serverEndpoint)
                                    .Replace("{{tenantId}}", id.Replace("-", ""))
                                    .Replace("{{subscriptionId}}", id.Replace("-", ""))
                                    .Replace("{{projectId}}", id.Replace("-", ""));
        }

        public static (string, int?) Extract(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                var targetString = source.Split('/', 2);
                int? sequentialNumber = null;
                if (targetString.Count() == 2)
                {
                    // found the sequential number
                    sequentialNumber = Convert.ToInt32(targetString[1]);
                    return (targetString[0], sequentialNumber);
                }
            }
            return (source, null);
        }
    }
}