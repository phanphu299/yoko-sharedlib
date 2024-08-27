using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class StartsWithOperationBuilder : ContainsOperationBuilder
    {
        protected override string OPERATION => "Starts with";

        public StartsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            value = value.EscapeLikeOperationSpecialCharacters();
            string sql = $"{fieldName.Replace(".ToLower()", "")} ilike concat(@startWithValue, '%')";
            return (sql, new object[] { value }, new string[] { "@startWithValue" });
        }
    }
}