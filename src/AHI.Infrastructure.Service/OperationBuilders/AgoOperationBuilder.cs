using System;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Enum;
using AHI.Infrastructure.Service.Model;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class AgoOperationBuilder : IOperationBuilder
    {
        protected string OPERATION = "Ago";

        protected IDictionary<PageSearchType, OperationBuilder> supportOperations;

        public AgoOperationBuilder()
        {
            supportOperations = new Dictionary<PageSearchType, OperationBuilder>
            {
                { PageSearchType.DATETIME2, BuildNullableDateTime2 },
                { PageSearchType.NULLABLE_DATETIME2, BuildNullableDateTime2 }
            };
        }

        public Tuple<string, object[], string[]> Build(FilterModel filter, Action<string[]> callback = null)
        {
            if (!supportOperations.ContainsKey(filter.QueryType)) throw new NotSupportedException($"{filter.QueryType} is not support for {OPERATION} operation");
            return supportOperations[filter.QueryType].Invoke(filter, callback);
        }

        /// <summary>
        /// Build Nullable DateTime with Ago Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableDateTime(FilterModel filter, Action<string[]> callback = null)
        {
            var values = filter.QueryValue.TrimStart('[').TrimEnd(']').Split(',');
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            var gap = int.Parse(values[1]);
            var compareField = values[0];
            if (values[0].Equals("now()"))
            {
                compareField = "DateTime.Now";
            }
            return BuildOperationSqlDateTime(filter.QueryKey, compareField, gap);
        }

        /// <summary>
        /// Build actual DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlDateTime(string fieldName, string compareField, int gap)
        {
            // TODO: should check datetime not nullable 
            string sql = $" ( {fieldName}.AddHours(@agoDate).date <= {compareField} && {compareField} <= {fieldName} )";
            return new Tuple<string, object[], string[]>(sql, new object[] { -gap }, new string[] { "@agoDate" });
        }

        /// <summary>
        /// Build actual Nullable DateTime query to database
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Tuple<string, object[], string[]> BuildOperationSqlNullableDateTime(string fieldName, string compareField, int gap)
        {
            // TODO: should check datetime not nullable 
            string sql = $" ( {fieldName}.Value.AddHours(@agoDate).date  <= {compareField}  && {compareField} <= {fieldName} ) ";
            return new Tuple<string, object[], string[]>(sql, new object[] { -gap }, new string[] { "@agoDate" });
        }

        /// <summary>
        /// Build Nullable DateTime2 with Ago Operation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected Tuple<string, object[], string[]> BuildNullableDateTime2(FilterModel filter, Action<string[]> callback = null)
        {
            var values = filter.QueryValue.TrimStart('[').TrimEnd(']').Split(',');
            if (values.Length != 2) throw new ArgumentException($"DateTime value should be an array of 2 values");
            var gap = int.Parse(values[1]);
            var compareField = values[0];
            if (values[0].Equals("now()"))
            {
                compareField = "DateTime.Now";
            }
            return BuildOperationSqlNullableDateTime(filter.QueryKey, compareField, gap);
        }
    }
}