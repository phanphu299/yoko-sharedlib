using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Parser
{
    public class NumbericParser : IValueParser<double>
    {
        public double Parse(string value)
        {
            return double.Parse(value);
        }
    }
}