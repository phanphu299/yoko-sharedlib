using System;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Parser
{
    public class GuidParser : IValueParser<Guid>
    {
        public Guid Parse(string value)
        {
            return Guid.Parse(value);
        }
    }
}