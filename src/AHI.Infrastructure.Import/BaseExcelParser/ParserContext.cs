using System.Collections.Generic;
using AHI.Infrastructure.Import.Abstraction;

namespace AHI.Infrastructure.Import.BaseExcelParser
{
    public class ParserContext : IParserContext
    {
        private string _templateExecutionDirectory;
        private string _timezoneOffset;
        public string _datetimeFormat;
        private IDictionary<string, object> _values;

        public string DateTimeFormat => _datetimeFormat;

        public string TimezoneOffset => _timezoneOffset;

        public ParserContext()
        {
            _values = new Dictionary<string, object>();
        }

        public void SetWorkingDirectory(string workingDir)
        {
            _templateExecutionDirectory = workingDir;
        }

        public void SetContextValue(string key, object format)
        {
            _values[key] = format;
        }

        public object GetContexValue(string key)
        {
            return _values.TryGetValue(key, out var result) ? result : null;
        }

        public void SetDateTimeFormat(string datetimeFormat)
        {
            _datetimeFormat = datetimeFormat;
        }

        public void SetTimezoneOffset(string timezoneOffset)
        {
            _timezoneOffset = timezoneOffset;
        }

    }
}