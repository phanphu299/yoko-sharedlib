using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using AHI.Infrastructure.SharedKernel.Extension;

namespace AHI.Infrastructure.MultiTenancy.Http.Handler
{
    public class ClientCrendetialAuthentication : DelegatingHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ClientCrendetialAuthentication> _logger;

        public ClientCrendetialAuthentication(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration, ILogger<ClientCrendetialAuthentication> logger)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await AccquireTokenAsync(request);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage result = null;
            try
            {
                result = await base.SendAsync(request, cancellationToken);
            }
            catch (System.Exception exc)
            {
                _logger.LogWarning($"Got exception {exc.Message}, system retry with new access token");
                accessToken = await AccquireTokenAsync(request, true);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                result = await base.SendAsync(request, cancellationToken);
            }
            return result;
        }

        private async Task<string> AccquireTokenAsync(HttpRequestMessage request, bool clearCache = false)
        {
            var key = "sa_access_token";
            var accessToken = _memoryCache.Get<string>(key);
            if (accessToken != null && clearCache == false)
                return accessToken;
            var clientId = _configuration["Authentication:ClientId"];
            var clientSecret = _configuration["Authentication:ClientSecret"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new System.Exception($"Authentication__ClientId is not configured in appsettings.");
            }
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new System.Exception($"Authentication__ClientSecret is not configured in appsettings.");
            }
            var content = new FormUrlEncodedContent(
            new List<KeyValuePair<string, string>>()
            {
                KeyValuePair.Create("grant_type","client_credentials"),
                KeyValuePair.Create("client_id",clientId),
                KeyValuePair.Create("client_secret",clientSecret)
            });
            var httpClient = _httpClientFactory.CreateClient("identity-service");
            var tokenRequest = await httpClient.PostAsync("connect/token", content);
            tokenRequest.EnsureSuccessStatusCode();
            var tokenResponseStream = await tokenRequest.Content.ReadAsByteArrayAsync();
            var response = tokenResponseStream.Deserialize<TokenResponse>();
            accessToken = response.AccessToken;
            _memoryCache.Set(key, accessToken, TimeSpan.FromSeconds(response.Expired - 300));
            return accessToken;
        }
    }
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int Expired { get; set; }
    }
}