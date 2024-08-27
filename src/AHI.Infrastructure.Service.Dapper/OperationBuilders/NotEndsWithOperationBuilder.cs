using System;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class NotEndsWithOperationBuilder : ContainsOperationBuilder
    {
        protected override string OPERATION => "Not ends with";

        public NotEndsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            value = value.EscapeLikeOperationSpecialCharacters();
            string sql = $"{fieldName.Replace(".ToLower()", "")} not ilike concat('%', @notEndwithsValue)";
            return (sql, new object[] { value }, new string[] { "@notEndwithsValue" });
        }
    }
}