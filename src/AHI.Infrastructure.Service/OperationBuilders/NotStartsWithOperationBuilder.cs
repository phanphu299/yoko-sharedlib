using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class NotStartsWithOperationBuilder : ContainsOperationBuilder
    {
        protected override string OPERATION => "Not starts with";

        public NotStartsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            string sql = $"{fieldName}.StartsWith(@notStartWithValue) == false";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@notStartWithValue" });
        }
    }
}