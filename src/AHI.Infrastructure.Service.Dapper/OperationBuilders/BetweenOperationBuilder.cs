using System;
using System.Linq;
using AHI.Infrastructure.Service.Dapper.Enum;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class BetweenOperationBuilder : BaseArrayBuilder
    {
        protected override string OPERATION => "Between";

        public BetweenOperationBuilder(IValueArrayParser<double> numbericParser,
                                       IValueArrayParser<DateTime> dateTimeParser) : base(numbericParser: numbericParser, dateTimeParser: dateTimeParser)
        {
            supportOperations.Clear();
            supportOperations.Add(QueryType.DATE, BuildDate);
            supportOperations.Add(QueryType.DATETIME, BuildDateTime);
        }


        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            if (values.Length != 2)
                throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"( @dateFrom <= {fieldName}::date && {fieldName}::date <= @dateTo )";
            return (sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            if (values.Length != 2)
                throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"( @dateFrom <= {fieldName} && {fieldName} <= @dateTo )";
            return (sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }
    }
}