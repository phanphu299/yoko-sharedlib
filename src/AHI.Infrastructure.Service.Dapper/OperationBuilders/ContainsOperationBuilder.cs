using System;
using AHI.Infrastructure.Service.Dapper.Enum;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class ContainsOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Contains";

        public ContainsOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
            supportOperations.Clear();
            supportOperations.Add(QueryType.TEXT, BuildText);
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            value = value.EscapeLikeOperationSpecialCharacters();
            string sql = $"{fieldName.Replace(".ToLower()", "")} ilike concat('%', @containValue, '%')";
            return (sql, new object[] { value }, new string[] { "@containValue" });
        }
    }
}