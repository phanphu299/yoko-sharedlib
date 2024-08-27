using System;
using System.Linq;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class NotInOperationBuilder : InOperationBuilder
    {
        protected override string OPERATION => "Not in";

        public NotInOperationBuilder(IValueArrayParser<string> stringParser,
                                    IValueArrayParser<double> numbericParser,
                                    IValueArrayParser<Guid> guidParser,
                                    IValueArrayParser<DateTime> dateTimeParser) : base(stringParser, numbericParser, guidParser, dateTimeParser)
        {
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string[] values)
        {
            var sqls = new List<string>();
            var sqlMap = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@notIn{i.ToString()}Value";
                sqls.Add($"{fieldName.Replace(".ToLower()", "")} <> {valueKey}");
                sqlMap.Add(valueKey);
            }
            return ($" ( {string.Join(" and ", sqls.ToArray())} )", values, sqlMap.ToArray());
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double[] values)
        {
            var sqls = new List<string>();
            var sqlMap = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@notIn{i.ToString()}Value";
                sqls.Add($"{fieldName} <> {valueKey}");
                sqlMap.Add(valueKey);
            }
            return ($" ( {string.Join(" and ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMap.ToArray());
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlGuid(string fieldName, Guid[] values)
        {
            var sqls = new List<string>();
            var sqlMap = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@notIn{i.ToString()}Value";
                sqls.Add($"{fieldName} <> {valueKey}");
                sqlMap.Add(valueKey);
            }
            return ($" ( {string.Join(" and ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMap.ToArray());
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@notIn{i.ToString()}Date";
                sqls.Add($"{fieldName}::date <> {valueKey}");
                sqlMapList.Add(valueKey);
            }
            return ($"( {string.Join(" and ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }

        protected override (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@notIn{i.ToString()}Date";
                sqls.Add($"{fieldName} <> {valueKey}");
                sqlMapList.Add(valueKey);
            }
            return ($"( {string.Join(" and ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
    }
}