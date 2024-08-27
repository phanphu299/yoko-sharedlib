using System;
using System.Linq;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class NotBetweenOperationBuilder : BetweenOperationBuilder
    {
        protected override string OPERATION => "Not between";

        public NotBetweenOperationBuilder(IValueArrayParser<double> numbericParser,
                                       IValueArrayParser<DateTime> dateTimeParser) : base(numbericParser: numbericParser, dateTimeParser: dateTimeParser)
        {
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double[] values)
        {
            if (values.Length != 2) throw new System.Exception("Value should be array which has length is 2");
            return new Tuple<string, object[], string[]>($"{fieldName} < @rangeValueFrom || {fieldName} > @rangeValueTo", values.Select(x => x as object).ToArray(), new string[] { "@rangeValueFrom", "@rangeValueTo" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"({fieldName}.date < @dateFrom.date || {fieldName}.date > @dateTo.date)";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"({fieldName}.Value.date < @dateFrom.date || {fieldName}.Value.date > @dateTo.date)";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"({fieldName} < @dateFrom || {fieldName} > @dateTo)";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime[] values)
        {
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            string sql = $"({fieldName}.Value < @dateFrom || {fieldName}.Value > @dateTo)";
            return new Tuple<string, object[], string[]>(sql, values.Select(x => x as object).ToArray(), new string[] { "@dateFrom", "@dateTo" });
        }
    }
}