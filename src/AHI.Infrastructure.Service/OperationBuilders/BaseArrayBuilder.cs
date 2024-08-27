using System;
using System.Linq;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Enum;
using AHI.Infrastructure.Service.Model;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public abstract class BaseArrayBuilder : IOperationBuilder
    {
        private IValueArrayParser<string> _stringParser;
        private IValueArrayParser<double> _numbericParser;
        private IValueArrayParser<Guid> _guidArrayParser;
        private IValueArrayParser<DateTime> _dateTimeParser;

        protected IDictionary<PageSearchType, OperationBuilder> supportOperations;

        protected abstract string OPERATION { get; }

        /// <summary>
        /// Do not support with default constructor
        /// </summary>
        public BaseArrayBuilder(IValueArrayParser<string> stringParser = null,
                                IValueArrayParser<double> numbericParser = null,
                                IValueArrayParser<Guid> guidArrayParser = null,
                                IValueArrayParser<DateTime> dateTimeParser = null)
        {
            _stringParser = stringParser;
            _numbericParser = numbericParser;
            _guidArrayParser = guidArrayParser;
            _dateTimeParser = dateTimeParser;
            supportOperations = new Dictionary<PageSearchType, OperationBuilder>
            {
                { PageSearchType.TEXT, BuildText },
                { PageSearchType.NUMBER, BuildNumber },
                { PageSearchType.GUID, BuildGuid },
                { PageSearchType.NULLABLE_GUID, BuildNullableGuid },
                { PageSearchType.DATE, BuildDate },
                { PageSearchType.NULLABLE_DATE, BuildNullableDate },
                { PageSearchType.DATETIME, BuildDateTime },
                { PageSearchType.NULLABLE_DATETIME, BuildNullableDateTime }
            };
        }

        /// <summary>
        /// Override interface method
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public virtual Tuple<string, object[], string[]> Build(FilterModel filter, Action<string[]> callback = null)
        {
            if (!supportOperations.ContainsKey(filter.QueryType)) throw new NotSupportedException($"{filter.QueryType} is not support for {OPERATION} operation");
            return supportOperations[filter.QueryType].Invoke(filter, callback);
        }

        #region Text
        /// <summary>
        /// Build Text value with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildText(FilterModel filter, Action<string[]> callback = null)
        {
            if (_stringParser == null) throw new InvalidOperationException($"{nameof(_stringParser)} is null");
            var textValues = _stringParser.Parse(filter.QueryValue);
            callback?.Invoke(textValues);
            return BuildOperationSqlText(filter.QueryKey, textValues);
        }

        /// <summary>
        /// Build actual Text query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@in{i.ToString()}Value";
                sqls.Add($"{index}.Equals({fieldName})");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($" ( {string.Join(" || ", sqls.ToArray())} )", values, sqlMapList.ToArray());
        }
        #endregion

        #region Number
        /// <summary>
        /// Build Number with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNumber(FilterModel filter, Action<string[]> callback = null)
        {
            if (_numbericParser == null) throw new InvalidOperationException($"{nameof(_numbericParser)} is null");
            var numbericValues = _numbericParser.Parse(filter.QueryValue);
            callback?.Invoke(numbericValues.Select(x => $"{x}").ToArray());
            return BuildOperationSqlNumber(filter.QueryKey, numbericValues);
        }

        /// <summary>
        /// Build actual Number query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double[] values)
        {
            var numbericOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@in{i.ToString()}Value";
                numbericOutput.Add($"{index} == {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" || ", numbericOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }


        #endregion

        #region Guid
        /// <summary>
        /// Build Guid value with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildGuid(FilterModel filter, Action<string[]> callback = null)
        {
            if (_guidArrayParser == null)
                throw new InvalidOperationException($"{nameof(_guidArrayParser)} is null");
            var guids = _guidArrayParser.Parse(filter.QueryValue);
            return BuildOperationSqlGuid(filter.QueryKey, guids);
        }

        /// <summary>
        /// Build actual Guid query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlGuid(string fieldName, Guid[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@in{i.ToString()}Value";
                sqls.Add($"{index} == {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" || ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion

        #region Nullable Guid
        /// <summary>
        /// Build Nullable Guid value with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableGuid(FilterModel filter, Action<string[]> callback = null)
        {
            if (_guidArrayParser == null)
                throw new InvalidOperationException($"{nameof(_guidArrayParser)} is null");
            var guids = _guidArrayParser.Parse(filter.QueryValue);
            return BuildOperationSqlNullableGuid(filter.QueryKey, guids);
        }

        /// <summary>
        /// Build actual Nullable Guid query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNullableGuid(string fieldName, Guid[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@in{i.ToString()}Value.Value";
                sqls.Add($"{index} == {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" || ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion

        #region Date
        /// <summary>
        /// Build Date with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildDate(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateTimeParser == null)
                throw new InvalidOperationException($"{nameof(_dateTimeParser)} is null");
            var dateTime = _dateTimeParser.Parse(filter.QueryValue);
            return BuildOperationSqlDate(filter.QueryKey, dateTime);
        }

        /// <summary>
        /// Build actual Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@{i.ToString()}";
                dateOutput.Add($"in{index}Date.date == {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" || ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion

        #region Nullable Date
        /// <summary>
        /// Build Nullable Date with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableDate(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateTimeParser == null)
                throw new InvalidOperationException($"{nameof(_dateTimeParser)} is null");
            var dateTime = _dateTimeParser.Parse(filter.QueryValue);
            return BuildOperationSqlNullableDate(filter.QueryKey, dateTime);
        }

        /// <summary>
        /// Build actual Nullable Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@{i.ToString()}";
                dateOutput.Add($"in{index}Date.Value.date == {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" || ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Build DateTime with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildDateTime(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateTimeParser == null)
                throw new InvalidOperationException($"{nameof(_dateTimeParser)} is null");
            var dateTime = _dateTimeParser.Parse(filter.QueryValue);
            return BuildOperationSqlDateTime(filter.QueryKey, dateTime);
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@{i.ToString()}";
                dateOutput.Add($"in{index}Date == {fieldName}");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" || ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion

        #region Nullable DateTime
        /// <summary>
        /// Build Nullable DateTime with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableDateTime(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateTimeParser == null)
                throw new InvalidOperationException($"{nameof(_dateTimeParser)} is null");
            var dateTime = _dateTimeParser.Parse(filter.QueryValue);
            return BuildOperationSqlNullableDateTime(filter.QueryKey, dateTime);
        }

        /// <summary>
        /// Build actual Nullable DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime[] values)
        {
            var dateOutput = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var index = $"@{i.ToString()}";
                dateOutput.Add($"in{index}Date.Value.date == {fieldName}.Value.Date");
                sqlMapList.Add(index);
            }
            return new Tuple<string, object[], string[]>($"( {string.Join(" || ", dateOutput.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion
    }
}