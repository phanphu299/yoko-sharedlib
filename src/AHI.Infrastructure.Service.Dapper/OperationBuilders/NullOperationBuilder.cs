using System;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Builder
{
    public class NullOperationBuilder : BaseBuilder
    {
        protected override string OPERATION => "Null";

        public NullOperationBuilder(
            IValueParser<string> stringParser,
            IValueParser<double> numbericParser,
            IValueParser<bool> boolParser,
            IValueParser<Guid> guidParser,
            IValueParser<DateTime> dateParser) : base(stringParser, numbericParser, boolParser, guidParser, dateParser)
        {
        }
    }
}