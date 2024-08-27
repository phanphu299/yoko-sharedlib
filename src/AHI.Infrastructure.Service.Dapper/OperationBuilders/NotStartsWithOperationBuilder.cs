using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class NotStartsWithOperationBuilder : ContainsOperationBuilder
    {
        protected override string OPERATION => "Not starts with";

        public NotStartsWithOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            value = value.EscapeLikeOperationSpecialCharacters();
            string sql = $"{fieldName.Replace(".ToLower()", "")} not ilike concat(@notStartWithValue, '%')";
            return (sql, new object[] { value }, new string[] { "@notStartWithValue" });
        }
    }
}