using System;
using System.Linq;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Dapper.Enum;
using AHI.Infrastructure.Service.Dapper.Model;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public abstract class BaseArrayBuilder : IOperationBuilder
    {
        private IValueArrayParser<string> _stringParser;
        private IValueArrayParser<double> _numbericParser;
        private IValueArrayParser<Guid> _guidArrayParser;
        private IValueArrayParser<DateTime> _dateTimeParser;

        protected IDictionary<QueryType, OperationBuilder> supportOperations;

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
            supportOperations = new Dictionary<QueryType, OperationBuilder>
            {
                { QueryType.TEXT, BuildText },
                { QueryType.NUMBER, BuildNumber },
                { QueryType.GUID, BuildGuid },
                { QueryType.DATE, BuildDate },
                { QueryType.DATETIME, BuildDateTime }
            };
        }

        /// <summary>
        /// Override interface method
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public virtual (string Query, object[] Values, string[] Tokens) Build(QueryFilter filter, Action<string[]> callback = null)
        {
            if (!supportOperations.ContainsKey(filter.QueryType))
                throw new NotSupportedException($"{filter.QueryType} is not support for {OPERATION} operation");
            return supportOperations[filter.QueryType].Invoke(filter, callback);
        }

        #region Text
        /// <summary>
        /// Build Text value with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildText(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_stringParser == null)
                throw new InvalidOperationException($"{nameof(_stringParser)} is null");
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
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@in{i.ToString()}Value";
                sqls.Add($"{fieldName} = {valueKey}");
                sqlMapList.Add(valueKey);
            }
            return ($" ( {string.Join(" or ", sqls.ToArray())} )", values, sqlMapList.ToArray());
        }
        #endregion

        #region Number
        /// <summary>
        /// Build Number with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildNumber(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_numbericParser == null)
                throw new InvalidOperationException($"{nameof(_numbericParser)} is null");
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
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@in{i.ToString()}Value";
                sqls.Add($"{fieldName} = {valueKey}");
                sqlMapList.Add(valueKey);
            }
            return ($"( {string.Join(" or ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }


        #endregion

        #region Guid
        /// <summary>
        /// Build Guid value with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildGuid(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_guidArrayParser == null)
                throw new InvalidOperationException($"{nameof(_guidArrayParser)} is null");
            var guidValues = _guidArrayParser.Parse(filter.QueryValue);
            return BuildOperationSqlGuid(filter.QueryKey, guidValues);
        }

        /// <summary>
        /// Build actual Guid query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlGuid(string fieldName, Guid[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@in{i.ToString()}Value";
                sqls.Add($"{fieldName} = {valueKey}");
                sqlMapList.Add(valueKey);
            }
            return ($"( {string.Join(" or ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion

        #region Date
        /// <summary>
        /// Build Date with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildDate(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_dateTimeParser == null)
                throw new InvalidOperationException($"{nameof(_dateTimeParser)} is null");
            var dateValue = _dateTimeParser.Parse(filter.QueryValue);
            return BuildOperationSqlDate(filter.QueryKey, dateValue);
        }

        /// <summary>
        /// Build actual Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@in{i.ToString()}Value";
                sqls.Add($"{fieldName}::date = {valueKey}");
                sqlMapList.Add(valueKey);
            }
            return ($"( {string.Join(" or ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Build DateTime with In Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildDateTime(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_dateTimeParser == null)
                throw new InvalidOperationException($"{nameof(_dateTimeParser)} is null");
            var dateTimeValue = _dateTimeParser.Parse(filter.QueryValue);
            return BuildOperationSqlDateTime(filter.QueryKey, dateTimeValue);
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime[] values)
        {
            var sqls = new List<string>();
            var sqlMapList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                var valueKey = $"@in{i.ToString()}Value";
                sqls.Add($"{fieldName} = {valueKey}");
                sqlMapList.Add(valueKey);
            }
            return ($"( {string.Join(" or ", sqls.ToArray())} )", values.Select(x => x as object).ToArray(), sqlMapList.ToArray());
        }
        #endregion
    }
}