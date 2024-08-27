using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Parser
{
    public class StringArrayParser : IValueArrayParser<string>
    {
        public string[] Parse(string value)
        {
            return value.TrimStart('[').TrimEnd(']').Split(',');
        }
    }
}