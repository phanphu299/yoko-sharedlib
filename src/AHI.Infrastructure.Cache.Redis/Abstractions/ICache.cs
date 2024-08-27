using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AHI.Infrastructure.Cache.Abstraction
{
    public interface ICache
    {
        Task StoreAsync<T>(string key, T value, int? database = null) where T : class;
        Task StoreAsync(string key, string value, int? database = null);
        Task StoreAsync<T>(string key, T value, TimeSpan duration, int? database = null) where T : class;
        Task StoreStringAsync(string key, string value, TimeSpan duration, int? database = null);
        Task<T> GetAsync<T>(string key, int? database = null) where T : class;
        Task<List<T>> GetByKeysAsync<T>(List<string> keyNames, int? database = null) where T : class;
        Task<string> GetStringAsync(string key, int? database = null);
        Task<List<string>> GetStringByKeysAsync(List<string> keyNames, int? database = null);
        Task<bool> DeleteAsync(string key, int? database = null);
        Task<bool> DeleteAllKeysAsync(string parternKey, int? database = null);
        Task<bool> DeleteAllKeysByBatchAsync(string parternKey, int batchSize = 50, int sleepInMiliseconds = 10, int? database = null);
        Task<T> GetHashByKeyAsync<T>(string hashKey, string hashField) where T : class;
        Task<T> GetHashByKeyAsync<T>(string hashKey, string hashField, int database) where T : class;
        Task<string> GetHashByKeyInStringAsync(string hashKey, string hashField);
        Task<string> GetHashByKeyInStringAsync(string hashKey, string hashField, int database);
        Task<List<T>> GetHashByKeysAsync<T>(string hashKey, List<string> hashFields) where T : class;
        Task<List<T>> GetHashByKeysAsync<T>(string hashKey, List<string> hashFields, int database) where T : class;
        Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value) where T : class;
        Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value, int database) where T : class;
        Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value, TimeSpan expiry) where T : class;
        Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value, TimeSpan expiry, int database) where T : class;
        Task SetHashByKeyAsync(string hashKey, string hashField, string value);
        Task SetHashByKeyAsync(string hashKey, string hashField, string value, int database);
        Task SetHashByKeyAsync(string hashKey, string hashField, string value, TimeSpan expiry);
        Task SetHashByKeyAsync(string hashKey, string hashField, string value, TimeSpan expiry, int database);
        Task<List<string>> GetHashFieldsByKeyAsync(string hashKey);
        Task<List<string>> GetHashFieldsByKeyAsync(string hashKey, int database);
        Task DeleteHashByKeyAsync(string hashKey, string hashField);
        Task DeleteHashByKeyAsync(string hashKey, string hashField, int database);
        Task DeleteHashByKeysAsync(string hashKey, List<string> hashFields);
        Task DeleteHashByKeysAsync(string hashKey, List<string> hashFields, int database);
        Task<bool> ClearHashAsync(string hashKey);
        Task<bool> ClearHashAsync(string hashKey, int database);
    }
}