using System;
using System.Linq;
using System.Text;
using AHI.Infrastructure.Service.Tag.Constant;
using AHI.Infrastructure.Service.Tag.Enum;
using AHI.Infrastructure.Service.Tag.Model;
using AHI.Infrastructure.SharedKernel.Models;

namespace AHI.Infrastructure.Service.Tag.Helper
{
    public static class DapperQueryHelper
    {
        /// <summary>
        /// Return query filter by tag
        /// </summary>
        /// <param name="pagingQuery">Normal query</param>
        /// <param name="searchAndFilter">CurrentSearchFilter</param>
        /// <param name="entityWildcard">Main table wildcard ex: select * from tableA tba that "tba" is wildcard</param>
        /// <param name="entityTableName">Main table name</param>
        /// <param name="entityTableIdName">Main table id name</param>
        /// <param name="entityIdType">type of entity id</param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static (string PagingQuery, SearchAndFilter SearchAndFilter) GenerateQueryByTagFilter(string pagingQuery, 
                                                                                                        SearchAndFilter searchAndFilter,
                                                                                                        SelectTableInfo selectTableInfo,
                                                                                                        EntityIdType entityIdType)
        {
            var tagAndQuery = searchAndFilter.And.Where(x =>
                       string.Compare(x.QueryKey, ServiceTagConstants.SEARCH_QUERY_KEY, StringComparison.InvariantCultureIgnoreCase) == 0
                       && x.Operation.StartsWith(nameof(LogicOperator.And), StringComparison.InvariantCultureIgnoreCase)
                       && string.Compare(x.QueryType, ServiceTagConstants.QUERY_TYPE, StringComparison.InvariantCultureIgnoreCase) == 0)
                       .ToArray();

            if (tagAndQuery.Any())
            {
                var joinFilterBuilder = new StringBuilder();

                for (int i = 0; i < tagAndQuery.Length; i++)
                {
                    var filter = tagAndQuery[i];
                    var normalOperator = filter.Operation.Contains("_") ? filter.Operation.Split('_', 2)[1].Trim() : string.Empty;
                    var distinctFilterValue = filter.QueryValue
                                       .Split(',')
                                       .Select(x => long.TryParse(x, out var tagId) ? tagId : -1) //If value cannot parse to long it will return -1 for next step
                                       .Distinct();

                    //If value contains -1 then all other tags does matter because they using "and" condition.
                    //This code force set = -1 for query alway return empty
                    if (!distinctFilterValue.Any() || distinctFilterValue.Any(x => x == -1))
                    {
                        joinFilterBuilder.Append(Environment.NewLine);
                        joinFilterBuilder = new StringBuilder($@"INNER JOIN ( SELECT {entityIdType} FROM entity_tags  WHERE 1 = 0 ) AS t ON m.id = t.{entityIdType}");
                        break;
                    }

                    var condition = $" = {distinctFilterValue.First()}";
                    var distinct = "";


                    if (distinctFilterValue.Count() > 1)
                    {
                        condition = $" IN ({string.Join(",", distinctFilterValue)})";
                        distinct = "DISTINCT";
                    }
                    joinFilterBuilder.Append(Environment.NewLine);
                    if (!string.IsNullOrEmpty(normalOperator) && ServiceTagConstants.ShowRecordDontHaveTagIfOperationNotMatch.Any(a => string.Compare(normalOperator, a, StringComparison.InvariantCultureIgnoreCase) == 0))
                    {
                        joinFilterBuilder.Append($@"INNER JOIN (
                                                    SELECT e.{selectTableInfo.EntityTablePrimaryColumn} as {entityIdType}
                                                    FROM {selectTableInfo.EntityTableName} e
                                                    LEFT JOIN entity_tags et ON e.id = et.{entityIdType}
                                                    GROUP BY e.id
                                                    HAVING COUNT(et.id) = 0 OR COUNT(CASE WHEN et.tag_id {condition} AND et.entity_type = '{selectTableInfo.EntityType}' THEN 1 END) > 0
                                                ) AS t{i} ON {selectTableInfo.SelectEntityWildcard}.{selectTableInfo.EntityTablePrimaryColumn} = t{i}.{entityIdType}");
                    }
                    else
                    {
                        joinFilterBuilder.Append($@"INNER JOIN (
                                                    SELECT {distinct} {entityIdType}
                                                    FROM entity_tags 
                                                    WHERE entity_type = '{selectTableInfo.EntityType}' AND tag_id {condition}
                                                ) AS t{i} ON {selectTableInfo.SelectEntityWildcard}.{selectTableInfo.EntityTablePrimaryColumn} = t{i}.{entityIdType}");
                    }
                }
                pagingQuery = $@"{pagingQuery}{Environment.NewLine}{joinFilterBuilder}";
            }

            var tagOrQuery = searchAndFilter.And.Where(x =>
                        string.Compare(x.QueryKey, ServiceTagConstants.SEARCH_QUERY_KEY, StringComparison.InvariantCultureIgnoreCase) == 0
                        && string.Compare(x.Operation, nameof(LogicOperator.Or), StringComparison.InvariantCultureIgnoreCase) == 0
                        && string.Compare(x.QueryType, ServiceTagConstants.QUERY_TYPE, StringComparison.InvariantCultureIgnoreCase) == 0)
                        .ToArray();


            if (tagOrQuery.Any())
            {
                var allFilterValue = tagOrQuery.First().QueryValue;
                var distinctFilterValue = allFilterValue
                                        .Split(',')
                                        .Select(x => long.TryParse(x, out var tagId) ? tagId : -1) //If value cannot parse to long it will return -1 for next step
                                        .Distinct();
                var orQuery = string.Empty;

                //If value contains -1 then all other tags does matter because they using "and" condition.
                //This code force set = -1 for query alway return empty
                if (!distinctFilterValue.Any() || distinctFilterValue.Any(x => x == -1))
                {
                    orQuery = $@"INNER JOIN ( SELECT {entityIdType} FROM entity_tags  WHERE 1 = 0 ) AS t ON m.id = t.{entityIdType}";
                }
                else
                {
                    var condition = $" = {distinctFilterValue.First()}";
                    var distinct = "";
                    if (distinctFilterValue.Count() > 1)
                    {
                        condition = $" IN ({string.Join(",", distinctFilterValue)})";
                        distinct = "DISTINCT";
                    }
                    orQuery = $@"INNER JOIN (SELECT {distinct} {entityIdType} 
                                    FROM entity_tags 
                                    WHERE entity_type = '{selectTableInfo.EntityType}' AND tag_id {condition}
                                ) AS tor ON m.id = tor.{entityIdType}";
                }
                pagingQuery = $@"{pagingQuery}{Environment.NewLine}{orQuery}";
            }

            var newFilter = new SearchAndFilter
            {
                And = searchAndFilter.And.Except(tagAndQuery).Except(tagOrQuery).ToList()
            };
            return (pagingQuery, newFilter);
        }
    }
}