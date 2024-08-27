using Newtonsoft.Json.Linq;

namespace AHI.Infrastructure.Service.Dapper.Helpers
{
    public static class JObjectHelper
    {
        /// <summary>
        /// Object can be: 
        /// {
        ///    "queryKey": null,
        ///    "queryValue": null,
        ///    "operation": null,
        ///    "queryType": null,
        ///    "and": null,
        ///    "or": [{data}]
        /// }
        /// </summary>
        /// <param name="filter">jObject filter</param>
        /// <returns>true if object has logical operator "and" or "or" value</returns>
        public static bool HasLogicalOperator(this JObject filter)
        {
            return filter.GetValue(Constant.LogicalOperator.AND)?.HasValues == true
                    || filter.GetValue(Constant.LogicalOperator.OR)?.HasValues == true;
        }
    }
}