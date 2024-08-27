using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Parser
{
    public class BoolParser : IValueParser<bool>
    {
        public bool Parse(string value)
        {
            return bool.Parse(value);
        }
    }
}