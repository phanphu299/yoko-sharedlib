using System;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class LessThanOrEqualsOperationBuilder : LessThanOperationBuilder
    {
        protected override string OPERATION => "Less than or equals";

        public LessThanOrEqualsOperationBuilder(IValueParser<double> numbericParser, IValueParser<DateTime> dateTimeParser) : base(numbericParser: numbericParser, dateTimeParser: dateTimeParser)
        {
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double value)
        {
            string sql = $"{fieldName} <= @lessThanOrEqualValue";
            return (sql, new object[] { value }, new string[] { "@lessThanOrEqualValue" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}::date <= @lessThanOrEqualDate";
            return (query, new object[] { value }, new string[] { "@lessThanOrEqualDate" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName} <= @lessThanOrEqualDate";
            return (dateSql, new object[] { value }, new string[] { "@lessThanOrEqualDate", });
        }
    }
}