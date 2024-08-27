using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.UserContext.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace AHI.Infrastructure.UserContext
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILoggerAdapter<UserContextMiddleware> _logger;
        //private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICache _cache;
        private readonly bool _useMemoryCache;
        private readonly int _cacheTimeout;
        private readonly IMemoryCache _memoryCache;
        private const string DEFAULT_CLAIM_IS_API_CLIENT = "apiClient";
        private const string DEFAULT_UPN_FOR_SYSTEM = "System";
        private const int DEFAULT_CACHE_TIMEOUT = 1;
        public UserContextMiddleware(RequestDelegate next, ICache cache, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _next = next;
            // _logger = logger;
            _cache = cache;
            _memoryCache = memoryCache;
            var cacheType = configuration["PROFILE_CACHE_TYPE"];
            _useMemoryCache = cacheType == "MEMORY_CACHE";

            var cacheTimeoutConfig = configuration["PROFILE_CACHE_TIMEOUT"];
            if (!int.TryParse(cacheTimeoutConfig, out _cacheTimeout))
            {
                _cacheTimeout = DEFAULT_CACHE_TIMEOUT;
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorize");
            }
            else
            {
                var upnClaim = context.User.Claims.FirstOrDefault(x => x.Type == "upn" || x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");
                if (upnClaim != null)
                {
                    var upn = upnClaim.Value;
                    var hash = upn.CalculateMd5Hash();
                    var key = $"user_upn_{hash}";
                    // only get from cache, no need to set because of the user-service will put into cache.
                    UserInfo userInfo = await GetAsync<UserInfo>(key);
                    if (userInfo == null)
                    {
                        var httpClientFactory = context.RequestServices.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
                        var httpClient = httpClientFactory.CreateClient("user-function");
                        var payload = await httpClient.PostAsync($"fnc/usr/users/info",
                            new StringContent($"{{\"Upn\" : \"{upn}\"}}", System.Text.Encoding.UTF8, "application/json")
                         );
                        payload.EnsureSuccessStatusCode();
                        var stream = await payload.Content.ReadAsByteArrayAsync();
                        userInfo = stream.Deserialize<UserInfo>();
                    }
                    var userContext = context.RequestServices.GetService(typeof(IUserContext)) as IUserContext;
                    userContext.SetId(userInfo.Id);
                    userContext.SetUpn(userInfo.Upn);
                    userContext.SetName(userInfo.FirstName, userInfo.MiddleName, userInfo.LastName);
                    userContext.SetDateTimeFormat(userInfo.DateTimeFormat);
                    userContext.SetTimezone(userInfo.TimezoneDto);
                    userContext.SetAvatar(userInfo.Avatar);
                    userContext.SetRightShorts(userInfo.RightShorts);
                    userContext.SetObjectRightShorts(userInfo.ObjectRightShorts);
                    SetApplicationId(userContext, context);
                }
                else
                {
                    var clientClaim = context.User.Claims.FirstOrDefault(x => x.Type == "client_id");
                    if (clientClaim != null)
                    {
                        var clientId = clientClaim.Value;
                        var key = $"client_id_{clientId}_info".ToLower();
                        // to optimize the performance of the system, for client info, we always using memory cache for this.
                        ClientInfo clientInfo = await GetAsync<ClientInfo>(key, forceUsingMemoryCache: true);
                        if (clientInfo == null)
                        {
                            // get the client information in the idp-service
                            var httpClientFactory = context.RequestServices.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
                            var idpService = httpClientFactory.CreateClient("identity-service");
                            idpService.DefaultRequestHeaders.Add(HeaderNames.Authorization, (IList<string>)context.Request.Headers[HeaderNames.Authorization]);
                            var response = await idpService.GetByteArrayAsync($"idp/clients/info?excludeUserContext=true");
                            clientInfo = response.Deserialize<ClientInfo>();
                            await SetAsync(key, clientInfo, TimeSpan.FromMinutes(_cacheTimeout), true);
                        }
                        if (clientInfo != null)
                        {
                            var isApiClient = context.User.Claims.Any(x => string.Equals(x.Type, DEFAULT_CLAIM_IS_API_CLIENT, StringComparison.InvariantCultureIgnoreCase)
                                                                        && string.Equals(x.Value, Boolean.TrueString, StringComparison.InvariantCultureIgnoreCase));
                            var userContext = context.RequestServices.GetService(typeof(IUserContext)) as IUserContext;
                            userContext.SetRightShorts(clientInfo.RightShorts);
                            userContext.SetObjectRightShorts(clientInfo.ObjectRightShorts);
                            //userContext.SetRightHashes(clientInfo.RightHashes);
                            if (isApiClient)
                            {
                                userContext.SetUpn(clientId); // Set Client Id as UPN to determine with Other UPN/ API Client
                                userContext.SetName(clientId, null, null);
                            }
                            else
                            {
                                userContext.SetUpn(DEFAULT_UPN_FOR_SYSTEM); // add default system as a Upn
                                userContext.SetName(DEFAULT_UPN_FOR_SYSTEM, null, null);
                            }
                            SetApplicationId(userContext, context);
                        }
                    }
                }
            }
            await _next(context);
        }
        private async Task<T> GetAsync<T>(string key, bool forceUsingMemoryCache = false) where T : class
        {
            if (_useMemoryCache || forceUsingMemoryCache)
            {
                return _memoryCache.Get<T>(key);
            }
            else
            {
                // use redis cache
                return await _cache.GetAsync<T>(key);
            }
        }
        private async Task SetAsync<T>(string key, T value, TimeSpan lifetime, bool forceUsingMemoryCache = false) where T : class
        {
            if (_useMemoryCache || forceUsingMemoryCache)
            {
                _memoryCache.Set(key, value, lifetime);
            }
            else
            {
                // use redis cache
                await _cache.StoreAsync(key, value, lifetime);
            }
        }

        private void SetApplicationId(IUserContext userContext, HttpContext context)
        {
            var applicationId = context.User.Claims.FirstOrDefault(x => x.Type == "applicationId");
            if (applicationId != null)
            {
                userContext.SetApplicationId(applicationId.Value);
            }
        }
    }

}
