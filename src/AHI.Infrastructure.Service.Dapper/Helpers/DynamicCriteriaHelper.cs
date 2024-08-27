using System;
using System.Dynamic;
using System.Linq;
using AHI.Infrastructure.Service.Dapper.Model;
using AHI.Infrastructure.SharedKernel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHI.Infrastructure.Service.Dapper.Helpers
{
    public static class DynamicCriteriaHelper
    {
        public static string ProcessDapperQuerySort<T>(string sortStr, T queryCriteria, Func<string, string, T, string> TransformSortByKey) where T : QueryCriteria
        {
            var sortItems = sortStr.Split(',');
            for (int i = 0; i < sortItems.Length; i++)
            {
                var sortBy = sortItems[i];
                var parts = sortBy.Split('=');
                var remapKeyToLower = parts[0];
                parts[0] = TransformSortByKey(remapKeyToLower, parts[1], queryCriteria);
                sortBy = $"{parts[0]}={parts[1]}";
                sortItems[i] = sortBy;
            }
            return string.Join(',', sortItems);
        }

        public static JObject ProcessDapperQueryFilter<T>(
            string filterString, JObject filter, T queryCriteria, Func<SearchFilter, T, SearchFilter> ConvertSearchFilter
        ) where T : QueryCriteria
        {
            //Add convert jobject normal key to camelcase. Ex FE using {"And": []} but BE catch filter["and"] only.
            filter = JObject.FromObject(filter.ToObject<ExpandoObject>(), SharedKernel.Extension.Constant.JsonSerializer);

            if (string.IsNullOrWhiteSpace(filterString))
                return null;
            bool processed = false;
            var andArr = filter[Constant.LogicalOperator.AND]?.ToArray();
            if (andArr?.Length > 0)
            {
                processed = true;
                ProcessConditions(filter, andArr, Constant.LogicalOperator.AND, filterString, queryCriteria, ConvertSearchFilter);
            }

            var orArr = filter[Constant.LogicalOperator.OR]?.ToArray();
            if (orArr?.Length > 0)
            {
                processed = true;
                ProcessConditions(filter, orArr, Constant.LogicalOperator.OR, filterString, queryCriteria, ConvertSearchFilter);
            }

            if (processed)
                filter = JObject.FromObject(filter);
            else
            {
                var singleFilterObj = JsonConvert.DeserializeObject<SearchFilter>(filterString);
                if (singleFilterObj.QueryKey != null)
                {
                    var searchFilter = ConvertSearchFilter(singleFilterObj, queryCriteria);
                    filter = searchFilter != null ? JObject.FromObject(searchFilter) : null;
                }
            }

            return filter.Properties().Any() ? filter : null;
        }

        private static void ProcessConditions<T>(
            JObject filter, JToken[] arr, string propertyName, string filterString,
            T queryCriteria, Func<SearchFilter, T, SearchFilter> ConvertSearchFilter
        ) where T : QueryCriteria
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var condition = arr[i] as JObject;

                //Case has logical operator
                if (condition.HasLogicalOperator())
                {
                    arr[i] = ProcessDapperQueryFilter(filterString, condition, queryCriteria, ConvertSearchFilter);
                }
                else
                {
                    var mappedCondition = MapComparison(arr[i].ToObject<SearchFilter>());
                    var searchFilter = ConvertSearchFilter(mappedCondition, queryCriteria);
                    arr[i] = searchFilter != null ? JObject.FromObject(searchFilter) : null;
                }
            }
            arr = arr.Where(item => item != null).ToArray();
            if (arr.Length > 0)
                filter[propertyName] = JArray.FromObject(arr);
            else
                filter.Property(propertyName).Remove();
        }

        private static SearchFilter MapComparison(SearchFilter condition)
        {
            var queryKeyNoSpaces = condition.QueryKey.Replace(" ", "");
            var isEqNull = queryKeyNoSpaces.EndsWith("==null");
            var isNeqNull = queryKeyNoSpaces.EndsWith("!=null");
            var isBoolType = string.Equals(condition.QueryType, Enum.QueryType.BOOLEAN.ToString(), StringComparison.OrdinalIgnoreCase);
            if (isBoolType && (isEqNull || isNeqNull))
            {
                var passedOperator = isEqNull ? "==" : "!=";
                var queryKey = condition.QueryKey.Substring(0, condition.QueryKey.LastIndexOf(passedOperator)).Trim();
                var isOriginalTrue = string.Equals(condition.QueryValue, "true", StringComparison.OrdinalIgnoreCase);
                var queryValue = isOriginalTrue;
                if (condition.Operation == Constant.Operation.NOT_EQUALS)
                    queryValue = !queryValue;
                if (isNeqNull)
                    queryValue = !queryValue;

                condition = new SearchFilter(
                    queryKey: queryKey,
                    queryValue: queryValue ? "true" : "false",
                    operation: Constant.Operation.NULL,
                    queryType: Enum.QueryType.NULL.ToString().ToLower()
                );
            }
            return condition;
        }
    }
}