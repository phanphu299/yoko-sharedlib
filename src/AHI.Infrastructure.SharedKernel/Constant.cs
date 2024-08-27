using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AHI.Infrastructure.SharedKernel.Extension
{
    public static class Constant
    {
        public static string DefaultDateTimeFormat = "yyyy-MM-ddTHH:mm:ss:ffff";

        static JsonSerializerSettings jss = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            DateFormatString = DefaultDateTimeFormat,
            DateParseHandling = DateParseHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public static JsonSerializerSettings JsonSerializerSetting
        {
            get
            {
                return jss;
            }
        }
        static JsonSerializer js = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            DateFormatString = DefaultDateTimeFormat,
            DateParseHandling = DateParseHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public static JsonSerializer JsonSerializer
        {
            get
            {
                return js;
            }
        }
    }
}