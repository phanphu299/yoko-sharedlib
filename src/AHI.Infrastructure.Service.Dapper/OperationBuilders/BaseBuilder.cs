using System;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Dapper.Enum;
using AHI.Infrastructure.Service.Dapper.Model;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public abstract class BaseBuilder : IOperationBuilder
    {
        private IValueParser<string> _stringParser;
        private IValueParser<double> _numbericParser;
        private IValueParser<bool> _boolParser;
        private IValueParser<DateTime> _dateTimeParser;
        private IValueParser<Guid> _guidParser;

        protected IDictionary<QueryType, OperationBuilder> supportOperations;

        protected abstract string OPERATION { get; }

        /// <summary>
        /// Do not support with default constructor
        /// </summary>
        public BaseBuilder(IValueParser<string> stringParser = null,
                        IValueParser<double> numbericParser = null,
                        IValueParser<bool> boolParser = null,
                        IValueParser<Guid> guidParser = null,
                        IValueParser<DateTime> dateTimeParser = null)
        {
            _stringParser = stringParser;
            _numbericParser = numbericParser;
            _boolParser = boolParser;
            _guidParser = guidParser;
            _dateTimeParser = dateTimeParser;
            supportOperations = new Dictionary<QueryType, OperationBuilder>
            {
                { QueryType.TEXT, BuildText },
                { QueryType.NUMBER, BuildNumber },
                { QueryType.BOOLEAN, BuildBoolean },
                { QueryType.GUID, BuildGuid },
                { QueryType.DATE, BuildDate },
                { QueryType.DATETIME, BuildDateTime },
                { QueryType.NULL, BuildNull }
            };
        }

        /// <summary>
        /// Override interface method
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public (string Query, object[] Values, string[] Tokens) Build(QueryFilter filter, Action<string[]> callback = null)
        {
            if (!supportOperations.ContainsKey(filter.QueryType))
                throw new NotSupportedException($"{filter.QueryType} is not support for {OPERATION} operation");
            return supportOperations[filter.QueryType].Invoke(filter, callback);
        }

        #region Null
        protected (string Query, object[] Values, string[] Tokens) BuildNull(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_boolParser == null)
                throw new InvalidOperationException($"{nameof(_boolParser)} is null");
            var isNull = _boolParser.Parse(filter.QueryValue);
            return BuildOperationSqlNull(filter.QueryKey, isNull);
        }

        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlNull(string fieldName, bool isNull)
        {
            var query = $"{fieldName.Replace(".ToLower()", "")} is {(isNull ? "" : "not")} null";
            return (query, new object[0], new string[0]);
        }
        #endregion

        #region Text
        /// <summary>
        /// Build Text value with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildText(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_stringParser == null)
                throw new InvalidOperationException($"{nameof(_stringParser)} is null");
            var textValue = _stringParser.Parse(filter.QueryValue);
            return BuildOperationSqlText(filter.QueryKey, textValue);
        }

        /// <summary>
        /// Build actual Text query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlText(string fieldName, string value)
        {
            var query = $"{fieldName.Replace(".ToLower()", "")} = @equalValue";
            return (query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Number
        /// <summary>
        /// Build Number with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildNumber(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_numbericParser == null)
                throw new InvalidOperationException($"{nameof(_numbericParser)} is null");
            var numbericValue = _numbericParser.Parse(filter.QueryValue);
            return BuildOperationSqlNumber(filter.QueryKey, numbericValue);
        }

        /// <summary>
        /// Build actual Number query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlNumber(string fieldName, double value)
        {
            var query = $"{fieldName} = @equalValue";
            return (query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Boolean
        /// <summary>
        /// Build Boolean with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildBoolean(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_boolParser == null)
                throw new InvalidOperationException($"{nameof(_boolParser)} is null");
            var value = _boolParser.Parse(filter.QueryValue);
            return BuildOperationSqlBoolean(filter.QueryKey, value);
        }

        /// <summary>
        /// Build actural date time query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlBoolean(string fieldName, bool value)
        {
            var query = $"{fieldName} = @equalValue";
            return (query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Guid
        /// <summary>
        /// Build Guid value with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected (string Query, object[] Values, string[] Tokens) BuildGuid(QueryFilter filter, Action<string[]> callback = null)
        {
            if (_guidParser == null)
                throw new InvalidOperationException($"{nameof(_guidParser)} is null");
            var guid = _guidParser.Parse(filter.QueryValue);
            return BuildOperationSqlGuid(filter.QueryKey, guid);
        }

        /// <summary>
        /// Build actual Guid query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlGuid(string fieldName, Guid value)
        {
            var query = $"{fieldName} = @equalValue";
            return (query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Date
        /// <summary>
        /// Build Date with Equals Operation
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
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}::date = @equalDate";
            return (query, new object[] { value }, new string[] { "@equalDate" });
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Build DateTime with Equals Operation
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
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual (string Query, object[] Values, string[] Tokens) BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            var query = $"{fieldName} = @equalDate";
            return (query, new object[] { value }, new string[] { "@equalDate" });
        }
        #endregion
    }
}