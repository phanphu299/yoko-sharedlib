using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Constants;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.MultiTenancy.Option;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.AspNetCore.Http;

namespace AHI.Infrastructure.MultiTenancy.Middleware
{
    public class MultiTenancyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerAdapter<MultiTenancyMiddleware> _logger;
        private readonly MultiTenancyOption _options;

        public MultiTenancyMiddleware(RequestDelegate next, ILoggerAdapter<MultiTenancyMiddleware> logger, MultiTenancyOption options)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.HasValue && _options.SkipPaths != null && _options.SkipPaths.Any())
            {
                var path = context.Request.Path.Value;
                var shouldSkip = _options.SkipPaths.Any(end => path.StartsWith(end));
                if (shouldSkip)
                {
                    _logger.LogTrace($"Skip getting information due to option setting for path: {path}");
                    return;
                }
            }
            if (_options.ExcludeWhenQueryPresentValues != null && _options.ExcludeWhenQueryPresentValues.Any() && context.Request.Query.Any())
            {
                var excludeChecks = (from q in context.Request.Query
                                     join e in _options.ExcludeWhenQueryPresentValues on q.Key equals e
                                     select q.Value[0]?.ToLowerInvariant()
                );
                if (excludeChecks.Contains("true"))
                {
                    _logger.LogTrace($"Skip getting information due to query: {string.Join(",", _options.ExcludeWhenQueryPresentValues)}");
                    return;
                }
            }
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                string traceId = Activity.Current.Context.TraceId.ToString();
                _logger.LogError($"Unauthorized - trace: {traceId}");
                return;
            }
            else
            {
                var tenantContext = context.RequestServices.GetService(typeof(ITenantContext)) as ITenantContext;
                // user authenticate
                var tenantId = GetHeaderAndClaimValue(context, HeaderConstant.TENANT_HEADER_KEY, "tenantId");
                if (string.IsNullOrEmpty(tenantId))
                {
                    context.Response.StatusCode = 403;
                    string traceId = Activity.Current.Context.TraceId.ToString();
                    _logger.LogError($"TenantId was not found - trace: {traceId}");
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync($"{{\"traceId\":\"{traceId}\"}}");
                    return;
                }
                tenantContext.SetTenantId(tenantId);
                var subscriptionId = GetHeaderAndClaimValue(context, HeaderConstant.SUBSCRIPTION_HEADER_KEY, "subscriptionId");
                if (string.IsNullOrEmpty(subscriptionId))
                {
                    context.Response.StatusCode = 403;
                    string traceId = Activity.Current.Context.TraceId.ToString();
                    _logger.LogError($"SubscriptionId was not found - trace: {traceId}");
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync($"{{\"traceId\":\"{traceId}\"}}");
                    return;
                }
                tenantContext.SetSubscriptionId(subscriptionId);
                var projectId = GetHeaderAndClaimValue(context, HeaderConstant.PROJECT_HEADER_KEY, "projectId");
                // projectId can be optional
                if (!string.IsNullOrEmpty(projectId))
                {
                    tenantContext.SetProjectId(projectId);
                }
            }
            await _next(context);
        }
        private string GetHeaderAndClaimValue(HttpContext context, string header, string claimName)
        {
            if (CanGetFromHeader(context))
            {
                var sourceId = TenantContextExtension.GetHeaderValue(context.Request.Headers, header);
                if (!string.IsNullOrEmpty(sourceId))
                {
                    return sourceId;
                }
            }
            // fallback to claim value
            return TenantContextExtension.GetClaimValue(context, claimName);
        }
        private bool CanGetFromHeader(HttpContext context)
        {
            return context.User.Claims.Any(x => x.Type == "allowHeader");
        }
    }
}