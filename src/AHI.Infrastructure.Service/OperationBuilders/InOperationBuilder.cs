using System;
using AHI.Infrastructure.Service.Abstraction;

namespace AHI.Infrastructure.Service.Builder
{
    public class InOperationBuilder : BaseArrayBuilder
    {
        protected override string OPERATION => "In";

        /// <summary>
        /// Do not support with default constructor
        /// </summary>
        private InOperationBuilder()
        {
        }

        /// <summary>
        /// Public constructor with default dependences
        /// </summary>
        /// <param name="stringParser"></param>
        /// <param name="numbericParser"></param>
        /// <param name="dateTimeParser"></param>
        public InOperationBuilder(IValueArrayParser<string> stringParser,
                                IValueArrayParser<double> numbericParser,
                                IValueArrayParser<Guid> guidArrayParser,
                                IValueArrayParser<DateTime> dateTimeParser) : base(stringParser, numbericParser, guidArrayParser, dateTimeParser)
        {
        }
    }
}