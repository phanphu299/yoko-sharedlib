using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Parser
{
    public class StringArrayParser : IValueArrayParser<string>
    {
        public string[] Parse(string value)
        {
            return value.TrimStart('[').TrimEnd(']').Split(',');
        }
    }
}