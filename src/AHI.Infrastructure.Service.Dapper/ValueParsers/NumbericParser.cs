using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Parser
{
    public class NumbericParser : IValueParser<double>
    {
        public double Parse(string value)
        {
            return double.Parse(value);
        }
    }
}