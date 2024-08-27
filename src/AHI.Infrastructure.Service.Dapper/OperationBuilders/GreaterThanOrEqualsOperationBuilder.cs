using System;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class GreaterThanOrEqualsOperationBuilder : GreaterThanOperationBuilder
    {
        protected override string OPERATION => "Greater than or equals";

        public GreaterThanOrEqualsOperationBuilder(IValueParser<double> numbericParser,
                                            IValueParser<DateTime> dateTimeParser) : base(numbericParser: numbericParser, dateTimeParser: dateTimeParser)
        {
        }

        /// <summary>
        /// Build actual Number query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double value)
        {
            string sql = $"{fieldName} >= @greaterThanOrEqualsValue";
            return (sql, new object[] { value }, new string[] { "@greaterThanOrEqualsValue" });
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName}::date >= @greaterThanOrEqualsDate";
            return (dateSql, new object[] { value }, new string[] { "@greaterThanOrEqualsDate" });
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName} >= @greaterThanOrEqualsDate";
            return (dateSql, new object[] { value }, new string[] { "@greaterThanOrEqualsDate" });
        }
    }
}