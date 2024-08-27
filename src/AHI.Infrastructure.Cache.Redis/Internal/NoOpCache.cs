using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Cache.Abstraction;

namespace AHI.Infrastructure.Cache.Redis
{
    public class NoOpCache : ICache
    {
        public Task<bool> DeleteAllKeysAsync(string parternKey, int? database = null)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAllKeysByBatchAsync(string parternKey, int batchSize = 50, int sleepInMiliseconds = 10, int? database = null)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(string key, int? database = null)
        {
            return Task.FromResult(true);
        }

        public Task<T> GetAsync<T>(string key, int? database = null) where T : class
        {
            return Task.FromResult<T>(default);
        }

        public Task<string> GetStringAsync(string key, int? database = null)
        {
            return Task.FromResult<string>(default);
        }

        public Task StoreAsync<T>(string key, T value, int? database = null) where T : class
        {
            return Task.CompletedTask;
        }

        public Task StoreAsync<T>(string key, T value, TimeSpan duration, int? database = null) where T : class
        {
            return Task.CompletedTask;
        }

        public Task StoreAsync(string key, string value, int? database = null)
        {
            return Task.CompletedTask;
        }

        public Task StoreStringAsync(string key, string value, TimeSpan duration, int? database = null)
        {
            return Task.CompletedTask;
        }

        Task<bool> ICache.ClearHashAsync(string hashKey)
        {
            return Task.FromResult(true);
        }

        Task<bool> ICache.ClearHashAsync(string hashKey, int database)
        {
            return Task.FromResult(true);
        }

        Task ICache.DeleteHashByKeyAsync(string hashKey, string hashField)
        {
            return Task.CompletedTask;
        }

        Task ICache.DeleteHashByKeyAsync(string hashKey, string hashField, int database)
        {
            return Task.CompletedTask;
        }

        Task ICache.DeleteHashByKeysAsync(string hashKey, List<string> hashFields)
        {
            return Task.CompletedTask;
        }

        Task ICache.DeleteHashByKeysAsync(string hashKey, List<string> hashFields, int database)
        {
            return Task.CompletedTask;
        }

        Task<List<T>> ICache.GetByKeysAsync<T>(List<string> keyNames, int? database)
        {
            return Task.FromResult<List<T>>(default);
        }

        Task<T> ICache.GetHashByKeyAsync<T>(string hashKey, string hashField)
        {
            return Task.FromResult<T>(default);
        }

        Task<T> ICache.GetHashByKeyAsync<T>(string hashKey, string hashField, int database)
        {
            return Task.FromResult<T>(default);
        }

        Task<string> ICache.GetHashByKeyInStringAsync(string hashKey, string hashField)
        {
            return Task.FromResult<string>(default);
        }

        Task<string> ICache.GetHashByKeyInStringAsync(string hashKey, string hashField, int database)
        {
            return Task.FromResult<string>(default);
        }

        Task<List<T>> ICache.GetHashByKeysAsync<T>(string hashKey, List<string> hashFields)
        {
            return Task.FromResult<List<T>>(default);
        }

        Task<List<T>> ICache.GetHashByKeysAsync<T>(string hashKey, List<string> hashFields, int database)
        {
            return Task.FromResult<List<T>>(default);
        }

        Task<List<string>> ICache.GetHashFieldsByKeyAsync(string hashKey)
        {
            return Task.FromResult<List<string>>(default);
        }

        Task<List<string>> ICache.GetHashFieldsByKeyAsync(string hashKey, int database)
        {
            return Task.FromResult<List<string>>(default);
        }

        Task<List<string>> ICache.GetStringByKeysAsync(List<string> keyNames, int? database)
        {
            return Task.FromResult<List<string>>(default);
        }

        Task ICache.SetHashByKeyAsync<T>(string hashKey, string hashField, T value)
        {
            return Task.CompletedTask;
        }

        Task ICache.SetHashByKeyAsync<T>(string hashKey, string hashField, T value, int database)
        {
            return Task.CompletedTask;
        }

        Task ICache.SetHashByKeyAsync<T>(string hashKey, string hashField, T value, TimeSpan expiry)
        {
            return Task.CompletedTask;
        }

        Task ICache.SetHashByKeyAsync<T>(string hashKey, string hashField, T value, TimeSpan expiry, int database)
        {
            return Task.CompletedTask;
        }

        Task ICache.SetHashByKeyAsync(string hashKey, string hashField, string value)
        {
            return Task.CompletedTask;
        }

        Task ICache.SetHashByKeyAsync(string hashKey, string hashField, string value, int database)
        {
            return Task.CompletedTask;
        }

        Task ICache.SetHashByKeyAsync(string hashKey, string hashField, string value, TimeSpan expiry)
        {
            return Task.CompletedTask;
        }

        Task ICache.SetHashByKeyAsync(string hashKey, string hashField, string value, TimeSpan expiry, int database)
        {
            return Task.CompletedTask;
        }
    }
}
