using AHI.Infrastructure.Service.Dapper.Enum;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class LQueryOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "LQuery";

        public LQueryOperationBuilder(IValueParser<string> stringParser) : base(stringParser)
        {
            supportOperations.Clear();
            supportOperations.Add(QueryType.TEXT, BuildText);
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            string sql = $"{fieldName.Replace(".ToLower()", "")} ~ concat(@LQueryValue)::lquery";
            return (sql, new object[] { value }, new string[] { "@LQueryValue" });
        }
    }
}