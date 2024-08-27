using Microsoft.AspNetCore.Mvc;

namespace AHI.Infrastructure.Exception.Filter
{
    public static class MiddlewareExtension
    {
        public static void ExceptionHandling(this MvcOptions options)
        {
            options.Filters.Add(typeof(GlobalExceptionFilter));
        }
    }
}