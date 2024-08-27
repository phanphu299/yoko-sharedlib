namespace AHI.Infrastructure.Import.Abstraction
{
    public interface IParserContext
    {
        void SetWorkingDirectory(string workingDir);
        void SetDateTimeFormat(string datetimeFormat);
        void SetTimezoneOffset(string timezoneOffset);
        void SetContextValue(string key, object value);
        object GetContexValue(string key);
        string DateTimeFormat { get; }
        string TimezoneOffset { get; }
    }
}