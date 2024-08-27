using System;
using System.Linq;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
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

        protected override Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string[] values)
        {
            var sqls = new List<string>();
            var sqlMap = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@notIn{i.ToString()}Value";
                sqls.Add($"{index} != {fieldName}");
                sqlMap.Add(index);
            }
            return new Tuple<string, object[], string[]>($" ( {string.Join(" && ", sqls.ToArray())} )", values, sqlMap.ToArray());
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double[] values)
        {
            var numbericOutput = new List<string>();
            var sqlMap = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@notIn{i.ToString()}Value";
                numbericOutput.Add($"{index} != {fieldName}");
                sqlMap.Add(index);
            }
            return new Tuple<string, object[], string[]>($" ( {string.Join(" && ", numbericOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMap.ToArray());
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlGuid(string fieldName, Guid[] values)
        {
            var sqls = new List<string>();
            var sqlMap = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@notIn{i.ToString()}Value";
                sqls.Add($"{index} != {fieldName}");
                sqlMap.Add(index);
            }
            return new Tuple<string, object[], string[]>($" ( {string.Join(" && ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMap.ToArray());
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@notIn{i.ToString()}Date";
                dateOutput.Add($"{index}.date != {fieldName}.date");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" && ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@notIn{i.ToString()}Date";
                dateOutput.Add($"{index}.date != {fieldName}.Value.date");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" && ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@notIn{i.ToString()}Date";
                dateOutput.Add($"{index} != {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" && ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@notIn{i.ToString()}Date";
                dateOutput.Add($"{index} != {fieldName}.Value");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" && ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }

        protected override Tuple<string, object[], string[]> BuildOperationSqlNullableGuid(string fieldName, Guid[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@in{i.ToString()}Value.Value";
                sqls.Add($"{index} != {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($" ( {string.Join(" || ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
    }
}