using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.SystemContext.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SystemContext.Enum;
using AHI.Infrastructure.SystemContext.Constants;

namespace AHI.Infrastructure.SystemContext
{
    public class SystemContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerAdapter<SystemContextMiddleware> _logger;
        public SystemContextMiddleware(RequestDelegate next, ILoggerAdapter<SystemContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var systemContext = context.RequestServices.GetService(typeof(ISystemContext)) as ISystemContext;
            // get the app level from header
            if (context.Request.Headers.TryGetValue(HeaderConstant.APP_LEVEL_HEADER_KEY, out var appLevelString))
            {
                _logger.LogTrace($"App level from header {appLevelString}");
                if (System.Enum.TryParse(appLevelString, out AppLevel appLevel))
                {
                    systemContext.SetAppLevel(appLevel);
                }
            }
            return _next(context);
        }
    }
}
