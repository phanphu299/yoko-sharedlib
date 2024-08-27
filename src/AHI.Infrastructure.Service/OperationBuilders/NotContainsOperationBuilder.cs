using System;
using AHI.Infrastructure.Service.Enum;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class NotContainsOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Not contains";

        public NotContainsOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
            supportOperations.Clear();
            supportOperations.Add(PageSearchType.TEXT, BuildText);
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            string sql = $"{fieldName}.CONTAINS(@notContainsValue) == false";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@notContainsValue" });
        }
    }
}