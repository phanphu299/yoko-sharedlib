using System;
using System.Linq;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class NotBetweenOperationBuilder : BetweenOperationBuilder
    {
        protected override string OPERATION => "Not between";

        public NotBetweenOperationBuilder(IValueArrayParser<double> numbericParser,
                                       IValueArrayParser<DateTime> dateTimeParser) : base(numbericParser: numbericParser, dateTimeParser: dateTimeParser)
        {
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double[] values)
        {
            if (values.Length != 2)
                throw new System.Exception("Value should be array which has length is 2");
            return ($"{fieldName} < @rangeValueFrom or {fieldName} > @rangeValueTo", values.Select(x => x as object).ToArray(), new string[] { "@rangeValueFrom", "@rangeValueTo" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            if (values.Length != 2)
                throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"({fieldName}::date < @dateFrom or {fieldName}::date > @dateTo)";
            return (sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            if (values.Length != 2)
                throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"({fieldName} < @dateFrom or {fieldName} > @dateTo)";
            return (sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }
    }
}