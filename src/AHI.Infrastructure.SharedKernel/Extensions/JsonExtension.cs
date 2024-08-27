using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHI.Infrastructure.SharedKernel.Extension
{
    public static class JsonExtension
    {

        static JsonSerializer defaultSerializer = new JsonSerializer()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            DateFormatString = Constant.DefaultDateTimeFormat,
            DateParseHandling = DateParseHandling.None,
        };
        public static T Deserialize<T>(this byte[] input)
        {
            T output = default;
            using (var mem = new MemoryStream(input))
            using (var sr = new StreamReader(mem))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                output = Constant.JsonSerializer.Deserialize<T>(jsonTextReader);
            }
            return output;
        }
        public static byte[] Serialize(this object value)
        {
            byte[] result = default;
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                Constant.JsonSerializer.Serialize(jsonWriter, value);
                jsonWriter.Flush();
                result = mem.ToArray();
            }
            return result;
        }
        public static string ToJson(this object value)
        {
            var array = Serialize(value);
            return Encoding.UTF8.GetString(array);
        }
        public static T FromJson<T>(this string jsonString)
        {
            var array = Encoding.UTF8.GetBytes(jsonString);
            return Deserialize<T>(array);
        }
        public static IDictionary<string, object> ParseJObject(string parentToken, JObject json, string delimiter = ".", bool parseJArray = true)
        {
            var result = new Dictionary<string, object>();
            foreach (var item in json)
            {
                var key = $"{parentToken}{delimiter}{item.Key}".Trim(delimiter.ToCharArray());
                if (item.Value is JObject o)
                {
                    var objectResult = ParseJObject(key, o);
                    MergeDictionary(objectResult, result);
                }
                else if (item.Value is JArray a && parseJArray)
                {
                    int index = 0;
                    foreach (var jitem in a)
                    {
                        var itemKey = $"{key}{delimiter}{index}".Trim(delimiter.ToCharArray());
                        index++;
                        if (jitem is JObject jo)
                        {
                            var objectResult = ParseJObject(itemKey, jo);
                            MergeDictionary(objectResult, result);
                        }
                        else if (jitem is JToken jt)
                        {
                            result[itemKey] = jitem.ToString();
                        }
                    }
                }
                else if (item.Value is JArray)
                {
                    result[item.Key] = item.Value;
                }
                else if (item.Value is JToken token)
                {
                    result[key] = token.ToString();
                }
            }
            return result;
        }
        private static void MergeDictionary(IDictionary<string, object> sources, IDictionary<string, object> target)
        {
            foreach (var item in sources)
            {
                target[item.Key] = item.Value;
            }
        }
    }
}