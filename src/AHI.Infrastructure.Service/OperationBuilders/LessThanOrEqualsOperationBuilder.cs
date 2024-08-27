using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class LessThanOrEqualsOperationBuilder : LessThanOperationBuilder
    {
        protected override string OPERATION => "Less than or equals";

        public LessThanOrEqualsOperationBuilder(IValueParser<double> numbericParser, IValueParser<DateTime> dateParser) : base(numbericParser: numbericParser, dateParser: dateParser)
        {
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double value)
        {
            string sql = $"{fieldName} <= @lessThanOrEqualValue";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@lessThanOrEqualValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.date <= @lessThanOrEqualDate.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@lessThanOrEqualDate" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.Value.date <= @lessThanOrEqualDate.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@lessThanOrEqualDate" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName} <= @lessThanOrEqualDate";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@lessThanOrEqualDate", });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.Value <= @lessThanOrEqualDate";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@lessThanOrEqualDate" });
        }
    }
}