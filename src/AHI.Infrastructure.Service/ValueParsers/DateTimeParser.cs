using System;
using System.Globalization;
using AHI.Infrastructure.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
namespace AHI.Infrastructure.Service.Parser
{
    public class DateTimeParser : IValueParser<DateTime>
    {
        public DateTime Parse(string value)
        {
            var date = DateTime.ParseExact(value, Constant.DefaultDateTimeFormat, CultureInfo.InvariantCulture);
            return date;
        }
    }
}