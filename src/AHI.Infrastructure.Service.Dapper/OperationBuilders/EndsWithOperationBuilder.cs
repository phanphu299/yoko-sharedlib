using System;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class EndsWithOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Ends with";

        public EndsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            value = value.EscapeLikeOperationSpecialCharacters();
            string sql = $"{fieldName.Replace(".ToLower()", "")} ilike concat('%', @endWithValue)";
            return (sql, new object[] { value }, new string[] { "@endWithValue" });
        }
    }
}