using AHI.Infrastructure.Service.Tag.Constant;
using AHI.Infrastructure.Service.Tag.Enum;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.SharedKernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AHI.Infrastructure.Service.Tag.Extension
{
    public static class CriteriaExtension
    {
        public static T MappingSearchTags<T>(this T criteria) where T : BaseCriteria
        {
            if (!string.IsNullOrEmpty(criteria.Filter))
            {
                var filters = criteria.Filter.FromJson<SearchFilter>();
                criteria.Filter = ReplaceFilter(filters).ToJson();
            }

            if (criteria.Fields != null && criteria.Fields.Length > 0 && !criteria.Fields.Any(x => x == ServiceTagConstants.TAG_SELECT_FIELD))
            {
                criteria.Fields = criteria.Fields.Append(ServiceTagConstants.TAG_SELECT_FIELD).ToArray();
            }

            return criteria;
        }

        private static SearchFilter ReplaceFilter(SearchFilter filter)
        {
            if (filter.Or != null)
            {
                foreach (var item in filter.Or)
                {
                    ReplaceFilter(item);
                }
            }
            else if (filter.And != null)
            {
                foreach (var item in filter.And)
                {
                    ReplaceFilter(item);
                }
            }
            else
            {
                LogicOperator? logical = null;
                var normalOperator = string.Empty;

                if (filter.Operation.StartsWith(nameof(LogicOperator.And), StringComparison.InvariantCultureIgnoreCase))
                {
                    logical = LogicOperator.And;
                    normalOperator = filter.Operation.Contains("_") ? filter.Operation.Split('_', 2)[1].Trim() : string.Empty;
                }

                if (string.Compare(filter.Operation, nameof(LogicOperator.Or), StringComparison.InvariantCultureIgnoreCase) == 0)
                    logical = LogicOperator.Or;

                if (!string.IsNullOrEmpty(filter.QueryKey)
                        && string.Compare(filter.QueryKey, ServiceTagConstants.SEARCH_QUERY_KEY, StringComparison.InvariantCultureIgnoreCase) == 0
                        && string.Compare(filter.QueryType, ServiceTagConstants.QUERY_TYPE, StringComparison.InvariantCultureIgnoreCase) == 0
                        && logical != null)
                {
                    if (string.IsNullOrEmpty(filter.QueryValue))
                    {
                        filter.QueryKey = $"(1 == 0)";
                        filter.QueryType = "boolean";
                        filter.Operation = "eq";
                        filter.QueryValue = "true";
                        return filter;
                    }

                    //Query key should be like this: "queryKey": "EntityTags.Any(a=>a.TagId==1 || a.TagId==2)"
                    var tagIds = filter.QueryValue.Split(",")
                                            .Where(x => !string.IsNullOrEmpty(x) && long.TryParse(x, out _))
                                            .ToList();

                    var joinOperator = string.Empty;

                    var joinTagIds = string.Join(",", tagIds);
                    var filterInit = $"EntityTags.Any(a => new long[] {{{joinTagIds}}}.Contains(a.TagId))";
                    var finalFilters = new List<string>() { filterInit };

                    switch (logical.Value)
                    {

                        case LogicOperator.And:
                            if (!string.IsNullOrEmpty(normalOperator) && ServiceTagConstants.ShowRecordDontHaveTagIfOperationNotMatch.Any(a => string.Compare(normalOperator, a, StringComparison.InvariantCultureIgnoreCase) == 0))
                            {
                                finalFilters.Add($"!EntityTags.Any()");
                            }
                            break;
                        case LogicOperator.Or:
                            break;
                    }

                    filter.QueryKey = $"({string.Join(" || ", finalFilters)})";
                    filter.QueryType = "boolean";
                    filter.Operation = "eq";
                    filter.QueryValue = "true";
                }
            }
            return filter;
        }
    }
}