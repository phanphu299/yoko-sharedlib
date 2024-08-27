using System;
using AHI.Infrastructure.Service.Enum;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class ContainsOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Contains";

        public ContainsOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
            supportOperations.Clear();
            supportOperations.Add(PageSearchType.TEXT, BuildText);
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            string sql = $"{fieldName}.CONTAINS(@containValue)";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@containValue" });
        }
    }
}