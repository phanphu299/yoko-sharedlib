using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.Validation.Model;
using AHI.Infrastructure.Validation.Repository.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using Microsoft.Extensions.Caching.Memory;
using AHI.Infrastructure.SystemContext.Abstraction;
using AHI.Infrastructure.Validation.Extension;

namespace AHI.Infrastructure.Validation.Repository
{
    public class SystemConfigRepository : IDynamicValidationRepository
    {
        #region Properties

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ITenantContext _tenantContext;
        private readonly ISystemContext _systemContext;

        private readonly ILoggerAdapter<SystemConfigRepository> _logger;

        private readonly IMemoryCache _cache;

        #endregion

        #region Constructor

        public SystemConfigRepository(IServiceProvider serviceProvider)
        {
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            _tenantContext = serviceProvider.GetService<ITenantContext>();
            _logger = serviceProvider.GetService<ILoggerAdapter<SystemConfigRepository>>();
            _cache = serviceProvider.GetService<IMemoryCache>();
            _systemContext = serviceProvider.GetService<ISystemContext>();
        }

        #endregion

        #region Methods

        public virtual async Task<IEnumerable<PropertyValidationRule>> GetValidationRulesAsync(
            IEnumerable<string> validationKeys)
        {
            var tasks = validationKeys.Select(GetValueAsync);
            return await Task.WhenAll(tasks);
        }

        protected virtual async Task<PropertyValidationRule> GetValueAsync(string key)
        {
            var cacheKey =
                $"{_tenantContext.TenantId}_{_tenantContext.SubscriptionId}_{_tenantContext.ProjectId}_{key}";
            var value = await GetFromCacheAsync(cacheKey);
            if (value != null)
            {
                _logger.LogDebug($"Cache hit {cacheKey}");
                return value;
            }

            var httpClient = _httpClientFactory.CreateClient("configuration-service", _tenantContext, _systemContext);
            var keys = GetKeys(key);
            var response = await httpClient.GetByteArrayAsync($"cnm/configs?key={string.Join(",", keys)}");
            var body = response.Deserialize<BaseSearchResponse<SystemConfigDto>>();
            var field = CreateFieldValidation(keys, body.Data);

            await StoreAsync(cacheKey, field);
            _logger.LogDebug($"Getting from api");
            return field;
        }

        protected virtual IEnumerable<string> GetKeys(string key)
        {
            return new[] { $"{key}.rule", $"{key}.description" };
        }

        protected virtual PropertyValidationRule CreateFieldValidation(IEnumerable<string> keys,
            IEnumerable<SystemConfigDto> configs)
        {
            var field = new PropertyValidationRule();
            foreach (var key in keys)
            {
                var lastDotIndex = key.LastIndexOf('.');
                var prefix = lastDotIndex < 0 ? key : key.Substring(0, lastDotIndex);
                field.Key = prefix;
                var config = configs.FirstOrDefault(x =>
                    string.Equals(key, x.Key, System.StringComparison.InvariantCultureIgnoreCase));
                if (key.EndsWith(".rule") && config != null)
                {
                    field.Regex = config.Value;
                }

                if (key.EndsWith(".description") && config != null)
                {
                    field.Description = config.Value;
                }
            }

            return field;
        }

        protected virtual Task<PropertyValidationRule> GetFromCacheAsync(string key)
        {
            if (_cache == null)
                return default;

            var cacheResult = _cache.Get<PropertyValidationRule>(key);
            return Task.FromResult(cacheResult);
        }

        protected virtual Task StoreAsync(string key, PropertyValidationRule value)
        {
            if (_cache != null)
            {
                _cache.Set(key, value);
            }
            return Task.CompletedTask;
        }

        #endregion
    }

    public class SystemConfigDto
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public SystemConfigDto(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}