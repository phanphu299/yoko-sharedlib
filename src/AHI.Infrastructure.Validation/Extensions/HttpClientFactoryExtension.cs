using System.Net.Http;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SystemContext.Abstraction;

namespace AHI.Infrastructure.Validation.Extension
{
    public static class HttpClientFactoryExtension
    {
        public static HttpClient CreateClient(
            this IHttpClientFactory httpClientFactory
            , string name
            , ITenantContext tenantContext
            , ISystemContext systemContext)
        {
            var httpClient = httpClientFactory.CreateClient(name, tenantContext);
            httpClient.DefaultRequestHeaders.Add(AHI.Infrastructure.SystemContext.Constants.HeaderConstant.APP_LEVEL_HEADER_KEY, systemContext.AppLevel.ToString());
            return httpClient;
        }
    }
}