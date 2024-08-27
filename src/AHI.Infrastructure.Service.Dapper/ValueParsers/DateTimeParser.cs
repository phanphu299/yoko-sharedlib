using System;
using System.Globalization;
using AHI.Infrastructure.Service.Dapper.Abstraction;
namespace AHI.Infrastructure.Service.Dapper.Parser
{
    public class DateTimeParser : IValueParser<DateTime>
    {
        public DateTime Parse(string value)
        {
            var date = DateTime.ParseExact(value, AHI.Infrastructure.SharedKernel.Extension.Constant.DefaultDateTimeFormat, CultureInfo.InvariantCulture);
            return date;
        }
    }
}