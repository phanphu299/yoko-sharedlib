using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class StartsWithOperationBuilder : ContainsOperationBuilder
    {
        protected override string OPERATION => "Starts with";

        public StartsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            string sql = $"{fieldName}.StartsWith(@startWithValue)";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@startWithValue" });
        }
    }
}