using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AHI.Infrastructure.SharedKernel.Models
{
    public class SearchFilter
    {
        public string QueryKey { get; set; }
        public string QueryValue { get; set; }
        public string Operation { get; set; }
        public string QueryType { get; set; }

        public List<SearchFilter> And { get; set; }
        public List<SearchFilter> Or { get; set; }

        public SearchFilter(string queryKey, string queryValue, string operation = "eq", string queryType = "text")
        {
            QueryKey = queryKey;
            QueryValue = queryValue;
            Operation = operation;
            QueryType = queryType;
        }

        public bool IsNotNull()
        {
            return string.IsNullOrEmpty(QueryValue)
                && string.IsNullOrEmpty(Operation)
                && string.IsNullOrEmpty(QueryType)
                && string.IsNullOrEmpty(QueryKey);
        }
    }

    public class FilteredSearchQuery
    {
        public bool ClientOverride => true;
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = ushort.MaxValue;
        public string Filter { get; set; } = string.Empty;

        public FilteredSearchQuery(params SearchFilter[] filterObjects) : this(LogicalOp.And, filterObjects)
        {
        }

        public FilteredSearchQuery(LogicalOp LogicOp, params SearchFilter[] filterObjects)
        {
            if (filterObjects.Length == 0)
                return;

            var filterObject = BuildMultiFilter(LogicOp, filterObjects);
            Filter = JsonConvert.SerializeObject(filterObject);
        }

        [JsonIgnore]
        public string AsJsonString => JsonConvert.SerializeObject(this);

        private static object BuildMultiFilter(LogicalOp LogicOp, SearchFilter[] filters)
        {
            if (LogicOp == LogicalOp.Or)
                return new { Or = filters };

            return new { And = filters };
        }

        public enum LogicalOp
        {
            And,
            Or
        }
    }

    public class SearchAndFilter
    {
        public List<SearchFilter> And { get; set; }
        public SearchAndFilter() { }
        public SearchAndFilter(List<SearchFilter> filters, string filterSecond)
        {
            if (string.IsNullOrEmpty(filterSecond) && filters.Count == 0)
                return;

            if (string.IsNullOrEmpty(filterSecond))
            {
                And = filters;
                return;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var f = JsonConvert.DeserializeObject<SearchFilter>(filterSecond, settings);
            if (!f.IsNotNull())
                filters.Add(f);
            else
            {
                var temp = JsonConvert.DeserializeObject<SearchAndFilter>(filterSecond, settings);
                if (temp.And != null && temp.And.Any())
                    filters.AddRange(temp.And);
            }
            And = filters;
        }
    }
}