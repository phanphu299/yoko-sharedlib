using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class EndsWithOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Ends with";

        public EndsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            string sql = $"{fieldName}.EndsWith(@endWithValue)";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@endWithValue" });
        }
    }
}