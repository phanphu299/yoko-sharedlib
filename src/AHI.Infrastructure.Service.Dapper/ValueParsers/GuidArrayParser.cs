using System;
using System.Linq;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Parser
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