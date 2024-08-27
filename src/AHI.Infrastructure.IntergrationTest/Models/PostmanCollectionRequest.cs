using System.Collections.Generic;
using Newtonsoft.Json;

namespace AHI.Infrastructure.IntegrationTest.Models
{
    public class PostmanCollectionRequest
    {
        public PostmanRequest Request { get; set; }
        [JsonProperty("event")]
        public IEnumerable<PostmanEvent> Events { get; set; }
    }
}