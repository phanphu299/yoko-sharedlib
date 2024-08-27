using System.Linq;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Parser
{
    public class NumbericArrayParser : IValueArrayParser<double>
    {
        public double[] Parse(string value)
        {
            var filterArray = value.TrimStart('[').TrimEnd(']').Split(',');
            var values = filterArray.Select(x => double.Parse(x.ToString())).ToArray();
            return values;
        }
    }
}