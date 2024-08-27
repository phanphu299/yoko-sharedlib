using System;
using AHI.Infrastructure.Service.Enum;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class LessThanOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Less than";

        public LessThanOperationBuilder(IValueParser<double> numbericParser,
            IValueParser<DateTime> dateParser) : base(numbericParser: numbericParser, dateParser: dateParser)
        {
            supportOperations.Clear();
            supportOperations.Add(PageSearchType.NUMBER, BuildNumber);
            supportOperations.Add(PageSearchType.DATE, BuildDate);
            supportOperations.Add(PageSearchType.NULLABLE_DATE, BuildNullableDate);
            supportOperations.Add(PageSearchType.DATETIME, BuildDateTime);
            supportOperations.Add(PageSearchType.NULLABLE_DATETIME, BuildNullableDateTime);
        }


        /// <summary>
        /// Build actual Number query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double value)
        {
            string sql = $"{fieldName} < @lessThanValue";
            return new Tuple<string, object[], string[]>(sql, new object[] { value }, new string[] { "@lessThanValue" });
        }

        /// <summary>
        /// Build actual Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName} < @lessThanDate.date";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@lessThanDate", });
        }

        /// <summary>
        /// Build actual Nullable Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName}.Value.date < @lessThanDate.date";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@lessThanDate", });
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName} < @lessThanDate";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@lessThanDate", });
        }

        /// <summary>
        /// Build actual Nullable DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime value)
        {
            string dateSql = $"{fieldName}.Value < @lessThanDate";
            return new Tuple<string, object[], string[]>(dateSql, new object[] { value }, new string[] { "@lessThanDate", });
        }
    }
}