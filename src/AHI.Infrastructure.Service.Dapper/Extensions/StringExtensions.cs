using System.Linq;
using System.Text.RegularExpressions;
using AHI.Infrastructure.Service.Dapper.Constant;

namespace AHI.Infrastructure.Service.Dapper.Extensions
{
    public static class StringExtensions
    {
        public static string ToStringQuote(this string text)
        {
            return $"\"{text}\"";
        }

        public static string EscapeLikeOperationSpecialCharacters(this string text)
        {
            return Regex.Replace(text, "([\\\\%_])", "\\$1");
        }

        public static string ConvertQueryKeyToLower(this string queryKey, bool convertToColumnStringName = true)
        {
            queryKey = queryKey.Trim();
            //Mapping for tolowercase column filter
            //Input: column1.ToLower() -> output: lower(column1)
            //Input: Column1.ToLower() -> output: lower(Column1)
            var queryRegex = "^(\\[?)([a-zA-Z0-9-_]+)(\\]?).[Tt]o[Ll]ower\\(\\)$";
            var regexMatch = Regex.Match(queryKey, queryRegex);
            if (!regexMatch.Success)
            {
                return convertToColumnStringName ? queryKey.ToColumnStringName() : queryKey;
            }
            var columnName = regexMatch.Groups[2].ToString();
            columnName = convertToColumnStringName ? columnName.ToColumnStringName() : columnName;
            return $"lower({columnName})";
        }

        public static string ToColumnStringName(this string queryKey)
        {
            //Case dynamic using "lower(column)" or "tableA.column1". Both case should return normal string not string quote.
            var column = QueryConstants.SKIP_STRING_QUOTE.Any(queryKey.Contains) ? queryKey : queryKey.ToStringQuote();
            return column;
        }
    }
}