using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Parser
{
    public class BoolParser : IValueParser<bool>
    {
        public bool Parse(string value)
        {
            return bool.Parse(value);
        }
    }
}