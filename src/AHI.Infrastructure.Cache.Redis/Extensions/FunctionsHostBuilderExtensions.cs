using System;
using System.Threading;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using AHI.Infrastructure.Cache.Redis.Constants;
using Microsoft.Extensions.Hosting;

namespace AHI.Infrastructure.Cache.Redis.Extension
{
    public static class FunctionsHostBuilderExtensions
    {
        public static void SetMinThread(this IFunctionsHostBuilder builder)
        {
            var minThread = builder.GetContext().Configuration["Dotnet:MinThreads"]?.ToInteger() ?? RedisConstant.DEFAULT_MIN_THREAD;
            ThreadPool.SetMinThreads(minThread, minThread);
        }
    } 
}