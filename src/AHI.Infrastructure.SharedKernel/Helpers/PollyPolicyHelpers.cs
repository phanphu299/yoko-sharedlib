using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.WaitAndRetry;

namespace AHI.Infrastructure.SharedKernel.Helper
{
    public static class PollyPolicyHelpers
    {
        private static readonly HttpStatusCode[] RETRY_STATUS_CODES =
        {
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        private static int DEFAULT_MAX_RETRIES = 3;
        private static int DEFAULT_DELAY = 1;
        private static int DEFAULT_MAX_RETRIES_CIRCUIT_BREAKER = 5;
        private static int DEFAULT_DELAY_CIRCUIT_BREAKER = 30;

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpClientRetryAsyncPolicy(IConfiguration configuration, IBaseLoggerAdapter logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static ISyncPolicy<HttpResponseMessage> GetHttpClientRetrySyncPolicy(IConfiguration configuration, IBaseLoggerAdapter logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpClientRetryAsyncPolicy(IConfiguration configuration, ILogger logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static ISyncPolicy<HttpResponseMessage> GetHttpClientRetrySyncPolicy(IConfiguration configuration, ILogger logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpClientRetryAsyncPolicy(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static ISyncPolicy<HttpResponseMessage> GetHttpClientRetrySyncPolicy(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpClientRetryAsyncPolicy(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for HttpClient response BadGateway/ServiceUnavailable/GatewayTimeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static ISyncPolicy<HttpResponseMessage> GetHttpClientRetrySyncPolicy(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    RETRY_STATUS_CODES.Contains(r.StatusCode))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Request: {exception.Result.RequestMessage.Method} {exception.Result.RequestMessage.RequestUri}, Response: {exception.Result.StatusCode}, Exception: {exception.Exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Circuit Breaker for HttpClient
        /// </summary>
        /// <param name="configuration">CircuitBreaker:MaxRetries for max retries. CircuitBreaker:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpClientCircuitBreakerAsyncPolicy(IConfiguration configuration)
        {
            int maxRetries = int.TryParse(configuration["CircuitBreaker:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES_CIRCUIT_BREAKER;
            int delayInSeconds = int.TryParse(configuration["CircuitBreaker:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY_CIRCUIT_BREAKER;

            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Polly Circuit Breaker for HttpClient
        /// </summary>
        /// <param name="configuration">CircuitBreaker:MaxRetries for max retries. CircuitBreaker:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static ISyncPolicy<HttpResponseMessage> GetHttpClientCircuitBreakerSyncPolicy(IConfiguration configuration)
        {
            int maxRetries = int.TryParse(configuration["CircuitBreaker:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES_CIRCUIT_BREAKER;
            int delayInSeconds = int.TryParse(configuration["CircuitBreaker:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY_CIRCUIT_BREAKER;

            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .CircuitBreaker(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Circuit Breaker Policy for HttpClient
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpClientCircuitBreakerAsyncPolicy(int maxRetries = 5, int delayInSeconds = 30)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Circuit Breaker Policy for HttpClient
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static ISyncPolicy<HttpResponseMessage> GetHttpClientCircuitBreakerSyncPolicy(int maxRetries = 5, int delayInSeconds = 30)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .CircuitBreaker(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetCommonRetryAsyncPolicy<T>(IConfiguration configuration, IBaseLoggerAdapter logger = null) where T : Exception
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetCommonRetrySyncPolicy<T>(IConfiguration configuration, IBaseLoggerAdapter logger = null) where T : Exception
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetCommonRetryAsyncPolicy<T>(IConfiguration configuration, ILogger logger = null) where T : Exception
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetCommonRetrySyncPolicy<T>(IConfiguration configuration, ILogger logger = null) where T : Exception
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetCommonRetryAsyncPolicy<T>(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null) where T : Exception
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetCommonRetrySyncPolicy<T>(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null) where T : Exception
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetCommonRetryAsyncPolicy<T>(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null) where T : Exception
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for specific exception
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetCommonRetrySyncPolicy<T>(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null) where T : Exception
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<T>(x => !(x is BrokenCircuitException))
                .OrInner<T>(x => !(x is BrokenCircuitException))
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    if (logger != null)
                        logger.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Circuit Breaker for specific exception
        /// </summary>
        /// <param name="configuration">CircuitBreaker:MaxRetries for max retries. CircuitBreaker:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static AsyncCircuitBreakerPolicy GetCommonCircuitBreakerPolicy<T>(IConfiguration configuration) where T : Exception
        {
            int maxRetries = int.TryParse(configuration["CircuitBreaker:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES_CIRCUIT_BREAKER;
            int delayInSeconds = int.TryParse(configuration["CircuitBreaker:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY_CIRCUIT_BREAKER;

            return Policy
                .Handle<T>()
                .CircuitBreakerAsync(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Polly Circuit Breaker for specific exception
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static AsyncCircuitBreakerPolicy GetCommonCircuitBreakerPolicy<T>(int maxRetries = 5, int delayInSeconds = 30) where T : Exception
        {
            return Policy
                .Handle<T>()
                .CircuitBreakerAsync(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetDbTimeoutRetryAsyncPolicy(IConfiguration configuration, IBaseLoggerAdapter logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetDbTimeoutRetrySyncPolicy(IConfiguration configuration, IBaseLoggerAdapter logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetDbTimeoutRetryAsyncPolicy(IConfiguration configuration, ILogger logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="configuration">Retry:MaxRetries for max retries. Retry:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetDbTimeoutRetrySyncPolicy(IConfiguration configuration, ILogger logger = null)
        {
            int maxRetries = int.TryParse(configuration["Retry:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES;
            int delayInSeconds = int.TryParse(configuration["Retry:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY;

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetDbTimeoutRetryAsyncPolicy(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetDbTimeoutRetrySyncPolicy(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.AsyncRetryPolicy GetDbTimeoutRetryAsyncPolicy(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetryAsync(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Retry Policy for Database connection timeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static Polly.Retry.RetryPolicy GetDbTimeoutRetrySyncPolicy(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(delayInSeconds), retryCount: maxRetries);
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .WaitAndRetry(delay, (exception, sleepDuration, retry, context) =>
                {
                    logger?.LogDebug($"Retry: {retry}, Exception: {exception?.Message}");
                });
        }

        /// <summary>
        /// Get Polly Circuit Breaker for Database connection timeout
        /// </summary>
        /// <param name="configuration">CircuitBreaker:MaxRetries for max retries. CircuitBreaker:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static AsyncCircuitBreakerPolicy GetDbTimeoutCircuitBreakerAsyncPolicy(IConfiguration configuration)
        {
            int maxRetries = int.TryParse(configuration["CircuitBreaker:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES_CIRCUIT_BREAKER;
            int delayInSeconds = int.TryParse(configuration["CircuitBreaker:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY_CIRCUIT_BREAKER;

            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .CircuitBreakerAsync(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Polly Circuit Breaker for Database connection timeout
        /// </summary>
        /// <param name="configuration">CircuitBreaker:MaxRetries for max retries. CircuitBreaker:DelayInSeconds for delay time. If setting not exist then using default value</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static CircuitBreakerPolicy GetDbTimeoutCircuitBreakerSyncPolicy(IConfiguration configuration)
        {
            int maxRetries = int.TryParse(configuration["CircuitBreaker:MaxRetries"], out maxRetries) ? maxRetries : DEFAULT_MAX_RETRIES_CIRCUIT_BREAKER;
            int delayInSeconds = int.TryParse(configuration["CircuitBreaker:DelayInSeconds"], out delayInSeconds) ? delayInSeconds : DEFAULT_DELAY_CIRCUIT_BREAKER;

            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .CircuitBreaker(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Polly Circuit Breaker for Database connection timeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static AsyncCircuitBreakerPolicy GetDbTimeoutCircuitBreakerAsyncPolicy(int maxRetries = 5, int delayInSeconds = 30)
        {
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .CircuitBreakerAsync(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }

        /// <summary>
        /// Get Polly Circuit Breaker for Database connection timeout
        /// </summary>
        /// <param name="maxRetries">Max retries</param>
        /// <param name="delayInSeconds">Delay time between retries</param>
        /// <param name="logger">Null by default. Show log when retry if logger not null</param>
        /// <returns></returns>
        public static CircuitBreakerPolicy GetDbTimeoutCircuitBreakerSyncPolicy(int maxRetries = 5, int delayInSeconds = 30)
        {
            return Policy
                .Handle<SocketException>()
                .Or<IOException>()
                .OrInner<SocketException>()
                .OrInner<IOException>()
                .CircuitBreaker(maxRetries, TimeSpan.FromSeconds(delayInSeconds));
        }
    }
}
