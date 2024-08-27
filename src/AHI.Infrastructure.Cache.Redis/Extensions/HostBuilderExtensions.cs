using System;
using System.Threading;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using AHI.Infrastructure.Cache.Redis.Constants;
using Microsoft.Extensions.Hosting;

namespace AHI.Infrastructure.Cache.Redis.Extension
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder SetMinThread(this IHostBuilder builder)
        {
            return builder.ConfigureServices((context, _) => 
            {
                var minThread = context.Configuration["Dotnet:MinThreads"]?.ToInteger() ?? RedisConstant.DEFAULT_MIN_THREAD;
                ThreadPool.SetMinThreads(minThread, minThread);
            });
        }
    } 
}