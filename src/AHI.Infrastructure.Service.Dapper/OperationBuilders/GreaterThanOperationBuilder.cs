using System;
using AHI.Infrastructure.Service.Dapper.Enum;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class GreaterThanOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Greater than";

        public GreaterThanOperationBuilder(IValueParser<double> numbericParser,
                                            IValueParser<DateTime> dateTimeParser) : base(numbericParser: numbericParser, dateTimeParser: dateTimeParser)
        {
            supportOperations.Clear();
            supportOperations.Add(QueryType.NUMBER, BuildNumber);
            supportOperations.Add(QueryType.DATE, BuildDate);
            supportOperations.Add(QueryType.DATETIME, BuildDateTime);
        }

        /// <summary>
        /// Build actual Number query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double value)
        {
            string sql = $"{fieldName} > @greaterThanValue";
            return (sql, new object[] { value }, new string[] { "@greaterThanValue" });
        }

        /// <summary>
        /// Build actual Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName}::date > @greaterThanDate";
            return (dateSql, new object[] { value }, new string[] { "@greaterThanDate" });
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName} > @greaterThanDate";
            return (dateSql, new object[] { value }, new string[] { "@greaterThanDate" });
        }
    }
}