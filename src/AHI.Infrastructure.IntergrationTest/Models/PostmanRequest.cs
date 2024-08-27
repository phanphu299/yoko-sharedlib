using System.Collections.Generic;

namespace AHI.Infrastructure.IntegrationTest.Models
{
    public class PostmanRequest
    {
        public PostmanUrl Url { get; set; }
        public string Method { get; set; }
        public List<HeaderData> Header { get; set; }
        public PostmanBody Body { get; set; }

    }
    public class PostmanUrl
    {
        public string Raw { get; set; }
        public string[] Path { get; set; }
    }

    public class PostmanBody
    {
        public string Mode { get; set; }
        public string Raw { get; set; }
        public PostmanFormBody[] Urlencoded { get; set; }
        public string[] Path { get; set; }
        public PostmanFormData[] FormData { get; set; }
    }
    public class PostmanFormData
    {
        public string Key { get; set; }
        public string Src { get; set; }
        public string Type { get; set; }
    }
    public class PostmanFormBody
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}