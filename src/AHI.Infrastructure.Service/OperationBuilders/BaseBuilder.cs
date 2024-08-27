using System;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Enum;
using AHI.Infrastructure.Service.Model;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public abstract class BaseBuilder : IOperationBuilder
    {
        private IValueParser<string> _stringParser;
        private IValueParser<double> _numbericParser;
        private IValueParser<bool> _boolParser;
        private IValueParser<DateTime> _dateParser;
        private IValueParser<Guid> _guidParser;

        protected IDictionary<PageSearchType, OperationBuilder> supportOperations;

        protected abstract string OPERATION { get; }

        /// <summary>
        /// Do not support with default constructor
        /// </summary>
        public BaseBuilder(IValueParser<string> stringParser = null,
                        IValueParser<double> numbericParser = null,
                        IValueParser<bool> boolParser = null,
                        IValueParser<Guid> guidParser = null,
                        IValueParser<DateTime> dateParser = null)
        {
            _stringParser = stringParser;
            _numbericParser = numbericParser;
            _boolParser = boolParser;
            _guidParser = guidParser;
            _dateParser = dateParser;
            supportOperations = new Dictionary<PageSearchType, OperationBuilder>
            {
                { PageSearchType.TEXT, BuildText },
                { PageSearchType.NUMBER, BuildNumber },
                { PageSearchType.BOOLEAN, BuildBoolean },
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
        public Tuple<string, object[], string[]> Build(FilterModel filter, Action<string[]> callback = null)
        {
            if (!supportOperations.ContainsKey(filter.QueryType)) throw new NotSupportedException($"{filter.QueryType} is not support for {OPERATION} operation");
            return supportOperations[filter.QueryType].Invoke(filter, callback);
        }

        #region Text
        /// <summary>
        /// Build Text value with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildText(FilterModel filter, Action<string[]> callback = null)
        {
            if (_stringParser == null) throw new InvalidOperationException($"{nameof(_stringParser)} is null");
            var textValue = _stringParser.Parse(filter.QueryValue);
            return BuildOperationSqlText(filter.QueryKey, textValue);
        }

        /// <summary>
        /// Build actual Text query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlText(string fieldName, string value)
        {
            var query = $"{fieldName}.Equals(@equalValue)";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Number
        /// <summary>
        /// Build Number with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNumber(FilterModel filter, Action<string[]> callback = null)
        {
            if (_numbericParser == null) throw new InvalidOperationException($"{nameof(_numbericParser)} is null");
            var numbericValue = _numbericParser.Parse(filter.QueryValue);
            return BuildOperationSqlNumber(filter.QueryKey, numbericValue);
        }

        /// <summary>
        /// Build actual Number query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNumber(string fieldName, double value)
        {
            var query = $"{fieldName} == @equalValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Boolean
        /// <summary>
        /// Build Boolean with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildBoolean(FilterModel filter, Action<string[]> callback = null)
        {
            if (_boolParser == null) throw new InvalidOperationException($"{nameof(_boolParser)} is null");
            var value = _boolParser.Parse(filter.QueryValue);
            return BuildOperationSqlBoolean(filter.QueryKey, value);
        }

        /// <summary>
        /// Build actural date time query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlBoolean(string fieldName, bool value)
        {
            var query = $"{fieldName} == @equalValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Guid
        /// <summary>
        /// Build Guid value with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildGuid(FilterModel filter, Action<string[]> callback = null)
        {
            if (_guidParser == null) throw new InvalidOperationException($"{nameof(_guidParser)} is null");
            var guid = _guidParser.Parse(filter.QueryValue);
            return BuildOperationSqlGuid(filter.QueryKey, guid);
        }

        /// <summary>
        /// Build actual Guid query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlGuid(string fieldName, Guid value)
        {
            var query = $"{fieldName} == @equalValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Nullable Guid
        /// Build Nullable Guid value with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableGuid(FilterModel filter, Action<string[]> callback = null)
        {
            if (_guidParser == null) throw new InvalidOperationException($"{nameof(_guidParser)} is null");
            var guid = _guidParser.Parse(filter.QueryValue);
            return BuildOperationSqlNullableGuid(filter.QueryKey, guid);
        }

        /// <summary>
        /// Build actual Guid query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNullableGuid(string fieldName, Guid value)
        {
            var query = $"{fieldName}.Value == @equalValue";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalValue" });
        }
        #endregion

        #region Date
        /// <summary>
        /// Build Date with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildDate(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateParser == null) throw new InvalidOperationException($"{nameof(_dateParser)} is null");
            var value = _dateParser.Parse(filter.QueryValue);
            return BuildOperationSqlDate(filter.QueryKey, value);
        }

        /// <summary>
        /// Build actual Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.Date == @equalDate.Date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalDate" });
        }
        #endregion

        #region Nullable Date
        /// <summary>
        /// Build Nullable Date with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableDate(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateParser == null) throw new InvalidOperationException($"{nameof(_dateParser)} is null");
            var value = _dateParser.Parse(filter.QueryValue);
            return BuildOperationSqlNullableDate(filter.QueryKey, value);
        }

        /// <summary>
        /// Build actual Nullable Date query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNullableDate(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.Value.date == @equalDate.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalDate" });
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Build DateTime with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildDateTime(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateParser == null) throw new InvalidOperationException($"{nameof(_dateParser)} is null");
            var value = _dateParser.Parse(filter.QueryValue);
            return BuildOperationSqlDateTime(filter.QueryKey, value);
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, DateTime value)
        {
            var query = $"{fieldName} == @equalDate";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalDate" });
        }
        #endregion

        #region Nullable DateTime
        /// <summary>
        /// Build Nullable DateTime with Equals Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableDateTime(FilterModel filter, Action<string[]> callback = null)
        {
            if (_dateParser == null) throw new InvalidOperationException($"{nameof(_dateParser)} is null");
            var value = _dateParser.Parse(filter.QueryValue);
            return BuildOperationSqlNullableDateTime(filter.QueryKey, value);
        }

        /// <summary>
        /// Build actual Nullable DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, DateTime value)
        {
            var query = $"{fieldName}.Value.date == @equalDate.date";
            return new Tuple<string, object[], string[]>(query, new object[] { value }, new string[] { "@equalDate" });
        }
        #endregion
    }
}