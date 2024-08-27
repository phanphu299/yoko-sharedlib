using System.Net.Http;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Constants;

namespace AHI.Infrastructure.MultiTenancy.Extension
{
    public static class HttpClientFactoryExtension
    {
        public static HttpClient CreateClient(
            this IHttpClientFactory httpClientFactory
            , string name
            , ITenantContext tenantContext)
        {
            var httpClient = httpClientFactory.CreateClient(name);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add(HeaderConstant.TENANT_HEADER_KEY, tenantContext.TenantId);
            httpClient.DefaultRequestHeaders.Add(HeaderConstant.SUBSCRIPTION_HEADER_KEY, tenantContext.SubscriptionId);
            if (!string.IsNullOrEmpty(tenantContext.ProjectId))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderConstant.PROJECT_HEADER_KEY, tenantContext.ProjectId);
            }
            return httpClient;
        }

    }
}