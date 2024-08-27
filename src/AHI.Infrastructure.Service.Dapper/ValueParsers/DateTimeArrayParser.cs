using System;
using System.Globalization;
using AHI.Infrastructure.Service.Dapper.Abstraction;

namespace AHI.Infrastructure.Service.Dapper.Parser
{
    public class DateTimeArrayParser : IValueArrayParser<DateTime>
    {
        public DateTime[] Parse(string value)
        {
            var filterArray = value.TrimStart('[').TrimEnd(']').Split(',');
            if (filterArray.Length != 2) throw new System.Exception($"Invalid input of type DateTime");
            DateTime fromDate = DateTime.ParseExact(filterArray[0].Trim(), AHI.Infrastructure.SharedKernel.Extension.Constant.DefaultDateTimeFormat, CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(filterArray[1].Trim(), AHI.Infrastructure.SharedKernel.Extension.Constant.DefaultDateTimeFormat, CultureInfo.InvariantCulture);
            return new DateTime[] { fromDate, toDate };
        }
    }
}