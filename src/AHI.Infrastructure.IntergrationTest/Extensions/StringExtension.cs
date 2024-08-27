using System;
using System.Text.RegularExpressions;
using AHI.Infrastructure.IntegrationTest.Models;
namespace AHI.Infrastructure.IntegrationTest.Extension
{
    public static class StringExtension
    {
        public static string ReplaceWithEnvironment(this string input, PostmanEnvironment environment)
        {
            var source = input;
            foreach (var value in environment.Values)
            {
                source = source.Replace($"{{{{{value.Key}}}}}", value.Value);
            }

            var keywords = new string[] { "$timestamp", "$randomDateRecent", "$guid", "$randomFirstName", "$randomEmail", "$randomUrl", "$randomInt" };
            foreach (var value in keywords)
            {
                source = Regex.Replace(source, $"{{{{\\{value}}}}}", (match) => GetNewValue(value));
            }
            return source;
        }
        
        private static string GetNewValue(string key)
        {
            var result = key switch
            {
                "$timestamp" => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                "$randomDateRecent" => DateTime.UtcNow.ToString("yyyy-MM-dd"),
                "$guid" => Guid.NewGuid().ToString(),
                "$randomFirstName" => Guid.NewGuid().ToString("N"),
                "$randomEmail" => $"{Guid.NewGuid().ToString("N")}@email.test",
                "$randomUrl" => $"https://{Guid.NewGuid().ToString("N")}.test",
                "$randomInt" => new Random().Next(100, 999).ToString()
            };
            return result;
        }
    }
}