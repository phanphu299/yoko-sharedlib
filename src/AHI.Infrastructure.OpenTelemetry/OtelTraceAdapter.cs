using System.Collections.Generic;
using System.Diagnostics;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.Extensions.Logging;

namespace AHI.Infrastructure.OpenTelemetry
{
    public class OtelTraceAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ActivitySource _serviceActivitySource;
        private readonly ILogger<T> _logger;
        public OtelTraceAdapter(ILogger<T> logger, ActivitySource activitySource)
        {
            _serviceActivitySource = activitySource;
            _logger = logger;
        }

        public void LogDebug(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                // AddTag("LogDebug", message, args);
                _logger.LogDebug(message, args);
            }
        }

        public void LogDebug(System.Exception exception, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                // AddTag("LogDebug", exception, message, args);
                _logger.LogDebug(exception, message, args);
            }

        }
        public void LogWarning(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                // AddTag("LogWarning", message, args);
                _logger.LogWarning(message, args);
            }
        }

        public void LogWarning(System.Exception exception, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                // AddTag("LogWarning", exception, message, args);
                _logger.LogWarning(exception, message, args);
            }
        }

        public void LogError(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                // AddTag("LogError", message, args);
                _logger.LogError(message, args);
            }
        }

        public void LogError(System.Exception exception, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                // AddTag("LogError", exception, message, args);
                _logger.LogError(exception, message, args);
            }
        }

        public void LogInformation(System.Exception exception, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                // AddTag("LogInformation", exception, message, args);
                _logger.LogInformation(exception, message, args);
            }
        }

        public void LogInformation(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                // AddTag("LogInformation", message, args);
                _logger.LogInformation(message, args);
            }
        }

        public void LogTrace(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                // AddTag("LogTrace", message, args);
                _logger.LogTrace(message, args);
            }
        }

        public void LogTrace(System.Exception exception, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                // AddTag("LogTrace", exception, message, args);
                _logger.LogTrace(exception, message, args);
            }
        }
        private void AddTag(string activityName, string message, params object[] args)
        {
            using (var activity = _serviceActivitySource.StartActivity(activityName))
            {
                activity?.SetTag(nameof(message), message);
                if (args != null && args.Length > 0 && args[0] is IDictionary<string, object> tags)
                {
                    foreach (var tag in tags)
                    {
                        activity?.SetTag(tag.Key, tag.Value);
                    }
                }

            }
        }
        // private void AddTag(string activityName, System.Exception exception, string message, params object[] args)
        // {
        //     using (var activity = _serviceActivitySource.StartActivity(activityName))
        //     {
        //         activity?.SetTag(nameof(message), message);
        //         activity?.SetTag("exceptionType", exception.GetType());
        //         activity?.SetTag("exceptionTrace", exception.StackTrace);
        //         activity?.SetTag("innerExceptionType", exception.InnerException?.GetType());
        //         activity?.SetTag("innerExceptionTrace", exception.InnerException?.StackTrace);
        //         activity?.SetTag("innerExceptionMessage", exception.InnerException?.Message);
        //         if (args != null && args.Length > 0 && args[0] is IDictionary<string, object> tags)
        //         {
        //             foreach (var tag in tags)
        //             {
        //                 activity?.SetTag(tag.Key, tag.Value);
        //             }
        //         }
        //     }
        // }
    }
}
