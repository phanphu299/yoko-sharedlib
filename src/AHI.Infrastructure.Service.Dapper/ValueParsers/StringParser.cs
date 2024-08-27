using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Parser
{
    public class StringParser : IValueParser<string>
    {
        public string Parse(string value)
        {
            return value;
        }
    }
}