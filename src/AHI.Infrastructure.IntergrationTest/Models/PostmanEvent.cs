using Newtonsoft.Json;

namespace AHI.Infrastructure.IntegrationTest.Models
{
    public class PostmanEvent
    {
        public string Listen { get; set; }
        public PostmanTestScript Script { get; set; }
        public string Type { get; set; }
    }
    public class PostmanTestScript
    {
        [JsonProperty("exec")]
        public string[] Code { get; set; }
    }
}