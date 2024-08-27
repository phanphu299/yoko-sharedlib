using AHI.Infrastructure.Service.Dapper.Enum;

namespace AHI.Infrastructure.Service.Dapper.Model
{
    public class QueryFilter
    {
        public string Operation { get; set; }
        public string QueryKey { get; set; }
        public QueryType QueryType { get; set; }
        public string QueryValue { get; set; }
    }
}