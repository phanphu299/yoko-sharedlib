using System;
using System.Linq;
using AHI.Infrastructure.Service.Enum;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class BetweenOperationBuilder : BaseArrayBuilder
    {
        protected override string OPERATION => "Between";

        public BetweenOperationBuilder(IValueArrayParser<double> numbericParser,
                                       IValueArrayParser<DateTime> dateTimeParser) : base(numbericParser: numbericParser, dateTimeParser: dateTimeParser)
        {
            supportOperations.Clear();
            supportOperations.Add(PageSearchType.NUMBER, BuildNumber);
            supportOperations.Add(PageSearchType.DATETIME, BuildDateTime);
            supportOperations.Add(PageSearchType.NULLABLE_DATETIME, BuildNullableDateTime);
            supportOperations.Add(PageSearchType.DATE, BuildDate);
            supportOperations.Add(PageSearchType.NULLABLE_DATE, BuildNullableDate);
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double[] values)
        {
            if (values.Length != 2) throw new System.Exception("Value should be array which has length is 2");
            return new Tuple<string, object[], string[]>($" @datefrom <= {fieldName}  && {fieldName} <= @dateto", values.Select(x => x as object).ToArray(), new string[] { "@datefrom", "@dateto" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"( @dateFrom.Date <= {fieldName}.Date && {fieldName}.Date <= @dateTo.Date )";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"( @dateFrom.Date <= {fieldName}.Value.Date && {fieldName}.Value.Date <= @dateTo.Date )";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"( @dateFrom <= {fieldName} && {fieldName} <= @dateTo )";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"( @dateFrom <= {fieldName}.Value && {fieldName}.Value <= @dateTo )";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }
    }
}