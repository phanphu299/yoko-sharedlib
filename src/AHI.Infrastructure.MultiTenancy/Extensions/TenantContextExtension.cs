using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Constants;
using AHI.Infrastructure.MultiTenancy.Internal;
using AHI.Infrastructure.SharedKernel.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AHI.Infrastructure.MultiTenancy.Extension
{
    public static class TenantContextExtension
    {
        public static ITenantContext RetrieveFromHeader(this ITenantContext tenantContext, HttpHeaders headers)
        {
            var tenantId = headers.GetValues(HeaderConstant.TENANT_HEADER_KEY).FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantContext.SetTenantId(tenantId);
            }

            var subscriptionId = headers.GetValues(HeaderConstant.SUBSCRIPTION_HEADER_KEY).FirstOrDefault();
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                tenantContext.SetSubscriptionId(subscriptionId);
            }
            if (headers.TryGetValues(HeaderConstant.PROJECT_HEADER_KEY, out var values))
            {
                var projectId = values.FirstOrDefault();
                if (!string.IsNullOrEmpty(projectId))
                {
                    tenantContext.SetProjectId(projectId);
                }
            }
            return tenantContext;
        }
        public static ITenantContext RetrieveFromHeader(this ITenantContext tenantContext, IDictionary<string, StringValues> httpRequestHeaders)
        {
            var tenantId = GetHeaderValue(httpRequestHeaders, HeaderConstant.TENANT_HEADER_KEY);
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantContext.SetTenantId(tenantId);
            }

            var subscriptionId = GetHeaderValue(httpRequestHeaders, HeaderConstant.SUBSCRIPTION_HEADER_KEY);
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                tenantContext.SetSubscriptionId(subscriptionId);
            }

            var projectId = GetHeaderValue(httpRequestHeaders, HeaderConstant.PROJECT_HEADER_KEY);
            if (!string.IsNullOrEmpty(projectId))
            {
                tenantContext.SetProjectId(projectId);
            }
            return tenantContext;
        }
        public static ITenantContext RetrieveFromHeader(this ITenantContext tenantContext, HttpContext httpContext)
        {
            var tenantId = GetHeaderValue(httpContext.Request.Headers, HeaderConstant.TENANT_HEADER_KEY);
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantContext.SetTenantId(tenantId);
            }

            var subscriptionId = GetHeaderValue(httpContext.Request.Headers, HeaderConstant.SUBSCRIPTION_HEADER_KEY);
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                tenantContext.SetSubscriptionId(subscriptionId);
            }

            var projectId = GetHeaderValue(httpContext.Request.Headers, HeaderConstant.PROJECT_HEADER_KEY);
            if (!string.IsNullOrEmpty(projectId))
            {
                tenantContext.SetProjectId(projectId);
            }
            return tenantContext;
        }
        public static ITenantContext RetrieveFromString(this ITenantContext tenantContext, string tenantId, string subscriptionId, string projectId = null)
        {
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantContext.SetTenantId(tenantId);
            }

            if (!string.IsNullOrEmpty(subscriptionId))
            {
                tenantContext.SetSubscriptionId(subscriptionId);
            }

            if (!string.IsNullOrEmpty(projectId))
            {
                tenantContext.SetProjectId(projectId);
            }
            return tenantContext;
        }
        public static ITenantContext Clone(this ITenantContext source)
        {
            var tenantContext = new TenantContext();
            tenantContext.SetTenantId(source.TenantId);
            tenantContext.SetSubscriptionId(source.SubscriptionId);
            tenantContext.SetProjectId(source.ProjectId);
            return tenantContext;
        }
        public static void CopyTo(this ITenantContext source, ITenantContext target)
        {
            target.SetTenantId(source.TenantId);
            target.SetSubscriptionId(source.SubscriptionId);
            target.SetProjectId(source.ProjectId);
        }
        public static string GetHeaderValue(IDictionary<string, StringValues> httpRequestHeaders, string header)
        {
            var valueFromHeader = httpRequestHeaders.TryGetValue(header, out var idString);
            if (valueFromHeader)
            {
                return idString;

            }
            return null;
        }
        public static string GetClaimValue(HttpContext context, string claimName)
        {
            var claim = context.User.Claims.FirstOrDefault(c => c.Type == claimName);
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }
        public static (Guid Id, int? SequentialNumber) Parse(string value)
        {
            var (targetIdString, sequentialNumber) = ConnectionStringExtension.Extract(value);
            if (!string.IsNullOrEmpty(targetIdString) && Guid.TryParse(targetIdString, out var targetId))
            {
                return (targetId, sequentialNumber);
            }
            return default;
        }
    }
}