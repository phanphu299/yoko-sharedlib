using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class NotEndsWithOperationBuilder : ContainsOperationBuilder
    {
        protected override string OPERATION => "Not ends with";

        public NotEndsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            string sql = $"{fieldName}.EndsWith(@notEndwithsValue) == false";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@notEndwithsValue" });
        }
    }
}