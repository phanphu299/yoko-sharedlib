using System.Collections.Generic;

namespace AHI.Infrastructure.IntegrationTest.Models
{
    public class PostmanCollection
    {
        public string Name { get; set; }
        public List<Request> Requests { get; set; }
    }
    public class Request
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string RawModeData { get; set; }
        public string Folder { get; set; }
        public List<HeaderData> HeaderData { get; set; }
        public int Order
        {
            get
            {
                switch (Method)
                {
                    case "POST":
                        return 1;
                    case "PUT":
                        return 2;
                    case "GET":
                        return 3;
                    case "DELETE":
                        return 4;
                }
                return 5;
            }
        }
    }
    public class HeaderData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Disabled { get; set; }
    }
}