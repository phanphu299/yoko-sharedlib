using AHI.Infrastructure.Service.Enum;

namespace AHI.Infrastructure.Service.Model
{
    public class FilterModel
    {
        public string QueryKey { get; set; }
        public PageSearchType QueryType { get; set; }
        public string QueryValue { get; set; }
        public string Operation { get; set; }
        public static FilterModel CreateFrom(string queryKey, PageSearchType queryType, string queryValue, string operation)
        {
            return new FilterModel()
            {
                QueryKey = queryKey,
                QueryType = queryType,
                QueryValue = queryValue,
                Operation = operation
            };
        }
    }
}