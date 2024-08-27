using System.Linq;
using System.Text;
using System.Dynamic;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Dapper.Model;
using AHI.Infrastructure.Service.Dapper.Abstraction;
using AHI.Infrastructure.Service.Dapper.Constant;
using Dapper;
using AHI.Infrastructure.Service.Dapper.Extensions;

namespace AHI.Infrastructure.Service.Dapper
{
    public class DapperQueryService : IQueryService
    {
        private readonly IFilterCompiler _filterCompiler;

        public DapperQueryService(IFilterCompiler filterCompiler)
        {
            _filterCompiler = filterCompiler;
        }

        public (string Query, ExpandoObject Value) CompileQuery(string query, QueryCriteria queryCriteria, bool paging = true)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.Append(query);

            var filterQuery = string.Empty;
            var value = new ExpandoObject();
            if (queryCriteria.Filter != null)
            {
                int count = 0;
                (filterQuery, value) = _filterCompiler.Compile(queryCriteria.Filter, ref count);
            }

            if (!string.IsNullOrEmpty(filterQuery))
            {
                queryBuilder.Append($" where {filterQuery}");
            }

            var sortQuery = BuidSortQuery(queryCriteria.Sorts);
            if (!string.IsNullOrEmpty(sortQuery))
            {
                queryBuilder.Append($" order by {sortQuery}");
            }

            if (paging)
            {
                var pagingQuery = $"limit {queryCriteria.PageSize} offset {queryCriteria.PageIndex * queryCriteria.PageSize}";
                queryBuilder.Append($" {pagingQuery}");
            }

            var resultQuery = queryBuilder.ToString();
            return (resultQuery, value);
        }

        public (string Paging, ExpandoObject Value) CompileQuery(SqlBuilder sqlBuilder, QueryCriteria queryCriteria, bool paging = true)
        {
            var value = new ExpandoObject();

            string filterQuery = null;
            if (queryCriteria.Filter != null)
            {
                int count = 0;
                (filterQuery, value) = _filterCompiler.Compile(queryCriteria.Filter, ref count);
            }

            if (!string.IsNullOrEmpty(filterQuery))
                sqlBuilder.Where($"({filterQuery})");

            var sortQuery = BuidSortQuery(queryCriteria.Sorts);
            if (!string.IsNullOrEmpty(sortQuery))
                sqlBuilder.OrderBy($"{sortQuery}");

            string pagingQuery = paging ? $"limit {queryCriteria.PageSize} offset {queryCriteria.PageIndex * queryCriteria.PageSize}" : null;

            return (pagingQuery, value);
        }

        public string BuidSortQuery(string sorts)
        {
            var resultQuery = string.Empty;
            if (!string.IsNullOrEmpty(sorts))
            {
                var sortList = new List<string>();
                var splitedSorts = sorts.Split(",");
                foreach (var splitedSort in splitedSorts)
                {
                    var splitedOrderBy = splitedSort.Split("=");
                    if (splitedOrderBy.Count() == 2)
                    {
                        var columnName = splitedOrderBy[0];
                        var orderByType = splitedOrderBy[1];

                        if (!Column.HasValidOrderByType(orderByType.ToLower()))
                        {
                            continue;
                        }

                        //Case dynamic using "lower(column)" or "tableA.column1". Both case should return normal string not string quote.
                        var column = columnName.ToColumnStringName();

                        sortList.Add($"{column} {orderByType}");
                    }
                }
                resultQuery = string.Join(",", sortList);
            }
            return resultQuery;
        }
    }
}