using System;
using System.Linq;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Parser
{
    public class GuidArrayParser : IValueArrayParser<Guid>
    {
        public Guid[] Parse(string value)
        {
            var array = value.TrimStart('[').TrimEnd(']').Split(',');
            return array.Select(x => Guid.Parse(x)).ToArray();
        }
    }
}