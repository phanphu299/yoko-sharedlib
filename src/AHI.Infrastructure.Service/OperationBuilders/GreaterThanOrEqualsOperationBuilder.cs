using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class GreaterThanOrEqualsOperationBuilder : GreaterThanOperationBuilder
    {
        protected override string OPERATION => "Greater than or equals";

        public GreaterThanOrEqualsOperationBuilder(IValueParser<double> numbericParser,
                                            IValueParser<DateTime> dateParser) : base(numbericParser: numbericParser, dateParser: dateParser)
        {
        }

        /// <summary>
        /// Build actual Number query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double value)
        {
            string sql = $"{fieldName} >= @greaterThanOrEqualsValue";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@greaterThanOrEqualsValue" });
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName}.date >= @greaterThanOrEqualsDate.date";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@greaterThanOrEqualsDate" });
        }

        /// <summary>
        /// Build actual Nullable Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName}.Value.date >= @greaterThanOrEqualsDate.date";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@greaterThanOrEqualsDate" });
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName} >= @greaterThanOrEqualsDate";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@greaterThanOrEqualsDate" });
        }

        /// <summary>
        /// Build actual date time query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName}.Value >= @greaterThanOrEqualsDate";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@greaterThanOrEqualsDate" });
        }
    }
}