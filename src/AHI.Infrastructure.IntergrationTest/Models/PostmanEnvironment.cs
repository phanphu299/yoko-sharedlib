using System.Collections.Generic;

namespace AHI.Infrastructure.IntegrationTest.Models
{
    public class PostmanEnvironment
    {
        public string Name { get; set; }
        public List<EnvironmentValue> Values { get; set; }
    }
    public class EnvironmentValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Enabled { get; set; }
        public EnvironmentValue()
        {

        }
        public EnvironmentValue(string key, string value, bool enable)
        {
            Key = key;
            Value = value;
            Enabled = enable;
        }
    }
}