using System;
using AHI.Infrastructure.Service.Dapper.Enum;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class NotContainsOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Not contains";

        public NotContainsOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
            supportOperations.Clear();
            supportOperations.Add(QueryType.TEXT, BuildText);
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            value = value.EscapeLikeOperationSpecialCharacters();
            string sql = $"{fieldName.Replace(".ToLower()", "")} not ilike concat('%', @notContainsValue, '%')";
            return (sql, new object[] { value }, new string[] { "@notContainsValue" });
        }
    }
}