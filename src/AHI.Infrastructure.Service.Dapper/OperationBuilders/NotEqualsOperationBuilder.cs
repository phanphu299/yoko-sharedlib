using System;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
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

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            var query = $"{fieldName.Replace(".ToLower()", "")} <> @notEqualsValue";
            return (query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double value)
        {
            var n = $"{fieldName} <> @notEqualsValue";
            return (n, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlBoolean(string fieldName, bool value)
        {
            var query = $"{fieldName} <> @notEqualsValue";
            return (query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlGuid(string fieldName, Guid value)
        {
            var query = $"{fieldName} <> @notEqualsValue";
            return (query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}::date <> @notEqualsValue";
            return (query, new object[] { value }, new string[] { "@notEqualsValue" });
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            var query = $"{fieldName} <> @notEqualsValue";
            return (query, new object[] { value }, new string[] { "@notEqualsValue" });
        }
    }
}