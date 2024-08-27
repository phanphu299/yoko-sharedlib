using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class NotEqualsOperationBuilder : EqualsOperationBuilder
    {
        protected override string OPERATION => "Not equals";

        public NotEqualsOperationBuilder(IValueParser<string> stringParser,
            IValueParser<double> numbericParser,
            IValueParser<bool> boolParser,
            IValueParser<Guid> guidParser,
            IValueParser<DateTime> dateParser) : base(stringParser, numbericParser, boolParser, guidParser, dateParser)
        {
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            var query = $"{fieldName} != @notEqualsValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double value)
        {
            var n = $"{fieldName} != @notEqualsValue";
            return new Tuple<string, object[], string[]>(n, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlBoolean(string fieldName, bool value)
        {
            var query = $"{fieldName} != @notEqualsValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlGuid(string fieldName, Guid value)
        {
            var query = $"{fieldName} != @notEqualsValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableGuid(string fieldName, Guid value)
        {
            var query = $"{fieldName}.Value != @notEqualsValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.date == @notEqualsValue.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.Value.date != @notEqualsValue.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.date != @notEqualsValue.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.Value.date != @notEqualsValue.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@notEqualsValue" });
        }
    }
}