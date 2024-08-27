namespace AHI.Infrastructure.SharedKernel.Abstraction
{
    public interface IBaseLoggerAdapter
    {
        void LogError(string message, params object[] args);
        void LogError(System.Exception exception, string message, params object[] args);
        void LogInformation(System.Exception exception, string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogTrace(string message, params object[] args);
        void LogTrace(System.Exception exception, string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogDebug(System.Exception exception, string message, params object[] args);
    }
}