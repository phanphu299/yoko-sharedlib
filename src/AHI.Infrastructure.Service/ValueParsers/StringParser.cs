using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Parser
{
    public class StringParser : IValueParser<string>
    {
        public string Parse(string value)
        {
            return value;
        }
    }
}