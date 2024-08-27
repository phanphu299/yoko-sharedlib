using System;
using System.Threading;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using AHI.Infrastructure.Cache.Redis.Constants;

namespace AHI.Infrastructure.Cache.Redis.Extension
{
    public static class StringExtensions
    {
        public static int ToInteger(this string value) => Convert.ToInt32(value);
    } 
}