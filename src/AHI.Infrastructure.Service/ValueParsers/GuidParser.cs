using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Parser
{
    public class GuidParser : IValueParser<Guid>
    {
        public Guid Parse(string value)
        {
            return Guid.Parse(value);
        }
    }
}