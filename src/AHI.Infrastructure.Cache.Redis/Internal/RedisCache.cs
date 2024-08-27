using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AHI.Infrastructure.Cache.Redis
{
    public class RedisCache : ICache
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private readonly ILoggerAdapter<RedisCache> _logger;
        private readonly CacheOptions _options;

        public RedisCache(CacheOptions options, ILoggerAdapter<RedisCache> logger)
        {
            _options = options;
            ConfigurationOptions configOptions;

            if (_options.ClusterEnabled)
            {
                configOptions = new ConfigurationOptions
                {
                    Password = _options.Password,
                    Ssl = _options.Ssl,
                    AbortOnConnectFail = _options.AbortOnConnectFail,
                    SyncTimeout = options.SyncTimeout,
                    AsyncTimeout = options.AsyncTimeout,
                    ConnectTimeout = options.ConnectTimeout
                };

                if (!string.IsNullOrEmpty(_options.EndPoints))
                {
                    // EndPoints: "server1:6379,server2:6379,server3:6379"
                    string[] endpointArray = _options.EndPoints.Split(',');
                    foreach (string endpoint in endpointArray)
                    {
                        configOptions.EndPoints.Add(endpoint);
                    }
                }
            }
            else
            {
                configOptions = new ConfigurationOptions
                {
                    EndPoints = { _options.Host },
                    Password = _options.Password,
                    Ssl = _options.Ssl,
                    AbortOnConnectFail = _options.AbortOnConnectFail,
                    DefaultDatabase = _options.Database,
                    SyncTimeout = options.SyncTimeout,
                    AsyncTimeout = options.AsyncTimeout,
                    ConnectTimeout = options.ConnectTimeout
                };
            }

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configOptions));
            _logger = logger;
        }

        ConnectionMultiplexer Connection => _lazyConnection.Value;

        public Task<bool> DeleteAsync(string key, int? database = null)
        {
            try
            {
                var databaseNumber = database ?? _options.Database;
                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    _logger.LogTrace($"Redis delete: {key}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    _logger.LogTrace($"Redis delete from db {databaseNumber}: {key}");
                }

                return redisDatabase.KeyDeleteAsync(key);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, exc.Message);
            }
            return Task.FromResult(false);
        }

        public async Task<T> GetAsync<T>(string key, int? database = null) where T : class
        {
            try
            {
                var databaseNumber = database ?? _options.Database;
                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    _logger.LogTrace($"Redis get: {key}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    _logger.LogTrace($"Redis get from db {databaseNumber}: {key}");
                }

                var value = await redisDatabase.StringGetAsync(key);
                if (!value.HasValue)
                {
                    return default;
                }
                var (objResult, success) = FromByteArray<T>(value);
                if (success)
                {
                    return objResult;
                }

                //fallback to legacy method
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, exc.Message);
            }
            return null;
        }

        public async Task<List<T>> GetByKeysAsync<T>(List<string> keyNames, int? database = null) where T : class
        {
            var databaseNumber = database ?? _options.Database;

            try
            {
                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    LogInformation($"Start Redis {nameof(GetByKeysAsync)}: {string.Join(",", keyNames)}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    LogInformation($"Start Redis {nameof(GetByKeysAsync)} from db {databaseNumber}: {string.Join(",", keyNames)}");
                }

                RedisValue[] cachedObjects = await redisDatabase.StringGetAsync(keyNames.Select(k => (RedisKey)k).ToArray());

                if (cachedObjects != null && cachedObjects.Length > 0)
                {
                    List<T> results = new List<T>();

                    foreach (RedisValue cachedObject in cachedObjects)
                    {
                        if (!cachedObject.IsNull)
                        {
                            var (objResult, success) = FromByteArray<T>(cachedObject);
                            if (success)
                            {
                                results.Add(objResult);
                            }
                            else
                            {
                                // Fallback to legacy method
                                results.Add(JsonConvert.DeserializeObject<T>(cachedObject.ToString()));
                            }
                        }
                    }

                    return results;
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            LogInformation($"End Redis {nameof(GetByKeysAsync)}: {string.Join(",", keyNames)}");
            return default;
        }

        public async Task<string> GetStringAsync(string key, int? database = null)
        {
            try
            {
                var databaseNumber = database ?? _options.Database;

                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    _logger.LogTrace($"Redis get string: {key}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    _logger.LogTrace($"Redis get string from db {databaseNumber}: {key}");
                }

                return await redisDatabase.StringGetAsync(key);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, exc.Message);
            }
            return null;
        }

        public async Task<List<string>> GetStringByKeysAsync(List<string> keyNames, int? database = null)
        {
            var databaseNumber = database ?? _options.Database;

            try
            {
                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    LogInformation($"Start Redis {nameof(GetStringByKeysAsync)}: {string.Join(",", keyNames)}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    LogInformation($"Start Redis {nameof(GetStringByKeysAsync)} from db {databaseNumber}: {string.Join(",", keyNames)}");
                }

                RedisValue[] cachedObjects = await redisDatabase.StringGetAsync(keyNames.Select(k => (RedisKey)k).ToArray());
                LogInformation($"End Redis {nameof(GetStringByKeysAsync)}: {string.Join(",", keyNames)}");

                if (cachedObjects != null && cachedObjects.Length > 0)
                {
                    List<string> results = new List<string>();

                    foreach (RedisValue cachedObject in cachedObjects)
                    {
                        results.Add(cachedObject);
                    }

                    return results;
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            LogInformation($"End Redis {nameof(GetStringByKeysAsync)}: {string.Join(",", keyNames)}");
            return default;
        }

        public async Task StoreAsync<T>(string key, T value, int? database = null) where T : class
        {
            try
            {
                var databaseNumber = database ?? _options.Database;
                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    _logger.LogTrace($"Redis store: {key}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    _logger.LogTrace($"Redis store to db {databaseNumber}: {key}");
                }

                await redisDatabase.StringSetAsync(key, ToByteArray<T>(value), TimeSpan.FromSeconds(_options.CacheDuration));
            }
            catch (System.Exception exc)
            {
                _logger.LogError(exc, exc.Message);
            }
        }

        public Task StoreAsync(string key, string value, int? database = null)
        {
            return StoreStringAsync(key, value, TimeSpan.FromSeconds(_options.CacheDuration), database);

        }

        public async Task StoreAsync<T>(string key, T value, TimeSpan duration, int? database = null) where T : class
        {
            try
            {
                var databaseNumber = database ?? _options.Database;
                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    _logger.LogTrace($"Redis store: {key}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    _logger.LogTrace($"Redis store to db {databaseNumber}: {key}");
                }

                await redisDatabase.StringSetAsync(key, ToByteArray<T>(value), duration);
            }
            catch (System.Exception exc)
            {
                _logger.LogError(exc, exc.Message);
            }
        }

        /// <summary>
        /// Delete all keys.
        /// This method doesn't support for Hash Key.
        /// </summary>
        /// <param name="parternKey">The parternKey.</param>
        /// <param name="database">The database.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> DeleteAllKeysAsync(string parternKey, int? database = null)
        {
            try
            {
                foreach (var ep in Connection.GetEndPoints())
                {
                    var server = Connection.GetServer(ep);
                    var targetDatabase = database ?? _options.Database;
                    var redisDatabase = Connection.GetDatabase(targetDatabase);
                    _logger.LogDebug($"Redis DeleteAllKeysByBatchAsync from db {targetDatabase} Page Size {_options.PageSize}");
                    var keys = server.Keys(targetDatabase, pattern: parternKey, _options.PageSize).ToArray();
                    if (keys.Any())
                    {
                        _logger.LogDebug($"Redis delete all keys from db {targetDatabase} ({string.Join(",", keys)})");
                        await redisDatabase.KeyDeleteAsync(keys, CommandFlags.FireAndForget);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting redis cache key");
                return false;
            }
        }

        /// <summary>
        /// Delete all keys by batch.
        /// This method doesn't support for Hash Key.
        /// </summary>
        /// <param name="parternKey">The parternKey.</param>
        /// <param name="batchSize">The batchSize.</param>
        /// <param name="sleepInMiliseconds">The sleepInMiliseconds.</param>
        /// <param name="database">The database.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> DeleteAllKeysByBatchAsync(string parternKey, int batchSize = 50, int sleepInMiliseconds = 10, int? database = null)
        {
            try
            {
                foreach (var ep in Connection.GetEndPoints())
                {
                    var server = Connection.GetServer(ep);
                    var targetDatabase = database ?? _options.Database;
                    var redisDatabase = Connection.GetDatabase(targetDatabase);
                    _logger.LogDebug($"Redis DeleteAllKeysByBatchAsync from db {targetDatabase} Page Size {_options.PageSize}");
                    var keys = server.Keys(targetDatabase, pattern: parternKey, _options.PageSize).ToArray();
                    if (keys.Any())
                    {
                        _logger.LogDebug($"Redis DeleteAllKeysByBatchAsync from db {targetDatabase} ({string.Join(",", keys)})");

                        int index = 0;
                        while (index * batchSize < keys.Count())
                        {
                            await redisDatabase.KeyDeleteAsync(keys.Skip(index * batchSize).Take(batchSize).ToArray(), CommandFlags.FireAndForget);
                            index++;
                            Thread.Sleep(sleepInMiliseconds);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redis DeleteAllKeysByBatchAsync");
                return false;
            }
        }

        public static byte[] ToByteArray<T>(T obj) where T : class
        {
            if (obj == null)
                return Array.Empty<byte>();
            return JsonExtension.Serialize(obj);
        }

        public static (T, bool) FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return (default(T), false);
            return (JsonExtension.Deserialize<T>(data), true);
        }

        public async Task StoreStringAsync(string key, string value, TimeSpan duration, int? database = null)
        {
            try
            {
                var databaseNumber = database ?? _options.Database;
                IDatabase redisDatabase;

                if (_options.ClusterEnabled)
                {
                    redisDatabase = Connection.GetDatabase();
                    _logger.LogTrace($"Redis string store: {key}");
                }
                else
                {
                    redisDatabase = Connection.GetDatabase(databaseNumber);
                    _logger.LogTrace($"Redis string store to db {databaseNumber}: {key}");
                }

                await redisDatabase.StringSetAsync(key, value, duration);
            }
            catch (System.Exception exc)
            {
                _logger.LogError(exc, exc.Message);
            }
        }

        public async Task<T> GetHashByKeyAsync<T>(string hashKey, string hashField) where T : class
        {
            LogInformation($"Start Redis {nameof(GetHashByKeyAsync)}: {hashKey}{hashField}");

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();
                RedisValue value = await redisDatabase.HashGetAsync(hashKey, hashField);

                if (!value.HasValue)
                {
                    return default;
                }

                var (objResult, success) = FromByteArray<T>(value);
                if (success)
                {
                    return objResult;
                }

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashByKeyAsync)}: {hashKey}{hashField}");
                return default;
            }
        }

        public async Task<T> GetHashByKeyAsync<T>(string hashKey, string hashField, int database) where T : class
        {
            var databaseNumber = database == -1 ? _options.Database : database;
            LogInformation($"Start Redis {nameof(GetHashByKeyAsync)} from db {databaseNumber}: {hashKey}{hashField}");

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);
                RedisValue value = await redisDatabase.HashGetAsync(hashKey, hashField);

                if (!value.HasValue)
                {
                    return default;
                }

                var (objResult, success) = FromByteArray<T>(value);
                if (success)
                {
                    return objResult;
                }

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashByKeyAsync)} from db {databaseNumber}: {hashKey}{hashField}");
                return default;
            }
        }

        public async Task<string> GetHashByKeyInStringAsync(string hashKey, string hashField)
        {
            LogInformation($"Start Redis {nameof(GetHashByKeyInStringAsync)}: {hashKey}{hashField}");

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();
                var result = await redisDatabase.HashGetAsync(hashKey, hashField);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashByKeyInStringAsync)}: {hashKey}{hashField}");
                return string.Empty;
            }
        }

        public async Task<string> GetHashByKeyInStringAsync(string hashKey, string hashField, int database)
        {
            var databaseNumber = database == -1 ? _options.Database : database;
            LogInformation($"Start Redis {nameof(GetHashByKeyInStringAsync)} from db {databaseNumber}: {hashKey}{hashField}");

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);
                var result = await redisDatabase.HashGetAsync(hashKey, hashField);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashByKeyInStringAsync)} from db {databaseNumber}: {hashKey}{hashField}");
                return string.Empty;
            }
        }

        public async Task<List<T>> GetHashByKeysAsync<T>(string hashKey, List<string> hashFields) where T : class
        {
            LogInformation($"Start Redis {nameof(GetHashByKeysAsync)}: {hashKey}{string.Join(", ", hashFields.ToArray())}");

            try
            {
                RedisValue[] redisValues = hashFields.Select(k => (RedisValue)k).ToArray();
                IDatabase redisDatabase = Connection.GetDatabase();
                RedisValue[] cachedObjects = await redisDatabase.HashGetAsync(hashKey, redisValues);

                if (cachedObjects != null && cachedObjects.Length > 0)
                {
                    List<T> results = new List<T>();

                    foreach (RedisValue cachedObject in cachedObjects)
                    {
                        if (!cachedObject.IsNull)
                        {
                            var (objResult, success) = FromByteArray<T>(cachedObject);
                            if (success)
                            {
                                results.Add(objResult);
                            }
                            else
                            {
                                // Fallback to legacy method
                                results.Add(JsonConvert.DeserializeObject<T>(cachedObject.ToString()));
                            }
                        }
                    }

                    return results;
                }

                return default;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashByKeysAsync)}: {hashKey}{string.Join(", ", hashFields.ToArray())}");
                return default;
            }
        }

        public async Task<List<T>> GetHashByKeysAsync<T>(string hashKey, List<string> hashFields, int database) where T : class
        {
            var databaseNumber = database == -1 ? _options.Database : database;
            LogInformation($"Start Redis {nameof(GetHashByKeysAsync)} from db {databaseNumber}: {hashKey}{string.Join(", ", hashFields.ToArray())}");

            try
            {
                RedisValue[] redisValues = hashFields.Select(k => (RedisValue)k).ToArray();
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);
                RedisValue[] cachedObjects = await redisDatabase.HashGetAsync(hashKey, redisValues);

                if (cachedObjects != null && cachedObjects.Length > 0)
                {
                    List<T> results = new List<T>();

                    foreach (RedisValue cachedObject in cachedObjects)
                    {
                        if (!cachedObject.IsNull)
                        {
                            var (objResult, success) = FromByteArray<T>(cachedObject);
                            if (success)
                            {
                                results.Add(objResult);
                            }
                            else
                            {
                                // Fallback to legacy method
                                results.Add(JsonConvert.DeserializeObject<T>(cachedObject.ToString()));
                            }
                        }
                    }

                    return results;
                }

                return default;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashByKeysAsync)} from db {databaseNumber}: {hashKey}{string.Join(", ", hashFields.ToArray())}");
                return default;
            }
        }

        public async Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value) where T : class
        {
            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();

                await redisDatabase.HashSetAsync(hashKey, hashField, ToByteArray<T>(value));
                await redisDatabase.KeyExpireAsync(hashKey, TimeSpan.FromSeconds(_options.CacheDuration));
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)}: {hashKey}{hashField}");
            }
        }

        public async Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value, int database) where T : class
        {
            var databaseNumber = database == -1 ? _options.Database : database;

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);

                await redisDatabase.HashSetAsync(hashKey, hashField, ToByteArray<T>(value));
                await redisDatabase.KeyExpireAsync(hashKey, TimeSpan.FromSeconds(_options.CacheDuration));
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)} from db {databaseNumber}: {hashKey}{hashField}");
            }
        }

        public async Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value, TimeSpan expiry) where T : class
        {
            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();

                await redisDatabase.HashSetAsync(hashKey, hashField, ToByteArray<T>(value));
                await redisDatabase.KeyExpireAsync(hashKey, expiry);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)}: {hashKey}{hashField}");
            }
        }

        public async Task SetHashByKeyAsync<T>(string hashKey, string hashField, T value, TimeSpan expiry, int database) where T : class
        {
            var databaseNumber = database == -1 ? _options.Database : database;

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);

                await redisDatabase.HashSetAsync(hashKey, hashField, ToByteArray<T>(value));
                await redisDatabase.KeyExpireAsync(hashKey, expiry);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)} from db {databaseNumber}: {hashKey}{hashField}");
            }
        }

        public async Task SetHashByKeyAsync(string hashKey, string hashField, string value)
        {
            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();

                await redisDatabase.HashSetAsync(hashKey, hashField, value);
                await redisDatabase.KeyExpireAsync(hashKey, TimeSpan.FromSeconds(_options.CacheDuration));
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)}: {hashKey}{hashField}");
            }
        }

        public async Task SetHashByKeyAsync(string hashKey, string hashField, string value, int database)
        {
            var databaseNumber = database == -1 ? _options.Database : database;

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);

                await redisDatabase.HashSetAsync(hashKey, hashField, value);
                await redisDatabase.KeyExpireAsync(hashKey, TimeSpan.FromSeconds(_options.CacheDuration));
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)} from db {databaseNumber}: {hashKey}{hashField}");
            }
        }

        public async Task SetHashByKeyAsync(string hashKey, string hashField, string value, TimeSpan expiry)
        {
            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();

                await redisDatabase.HashSetAsync(hashKey, hashField, value);
                await redisDatabase.KeyExpireAsync(hashKey, expiry);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)}: {hashKey}{hashField}");
            }
        }

        public async Task SetHashByKeyAsync(string hashKey, string hashField, string value, TimeSpan expiry, int database)
        {
            var databaseNumber = database == -1 ? _options.Database : database;

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);

                await redisDatabase.HashSetAsync(hashKey, hashField, value);
                await redisDatabase.KeyExpireAsync(hashKey, expiry);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(SetHashByKeyAsync)} from db {databaseNumber}: {hashKey}{hashField}");
            }
        }

        public async Task<List<string>> GetHashFieldsByKeyAsync(string hashKey)
        {
            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();
                RedisValue[] fields = await redisDatabase.HashKeysAsync(hashKey);
                return fields.Select(x => x.ToString()).ToList();
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashFieldsByKeyAsync)}: {hashKey}");
                return default;
            }
        }

        public async Task<List<string>> GetHashFieldsByKeyAsync(string hashKey, int database)
        {
            var databaseNumber = database == -1 ? _options.Database : database;

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);
                RedisValue[] fields = await redisDatabase.HashKeysAsync(hashKey);
                return fields.Select(x => x.ToString()).ToList();
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(GetHashFieldsByKeyAsync)} from db {databaseNumber}: {hashKey}");
                return default;
            }
        }

        public async Task DeleteHashByKeyAsync(string hashKey, string hashField)
        {
            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();
                await redisDatabase.HashDeleteAsync(hashKey, hashField);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(DeleteHashByKeyAsync)}: {hashKey}{hashField}");
            }
        }

        public async Task DeleteHashByKeyAsync(string hashKey, string hashField, int database)
        {
            var databaseNumber = database == -1 ? _options.Database : database;

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);
                await redisDatabase.HashDeleteAsync(hashKey, hashField);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(DeleteHashByKeyAsync)} from db {databaseNumber}: {hashKey}{hashField}");
            }
        }

        public async Task DeleteHashByKeysAsync(string hashKey, List<string> hashFields)
        {
            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();
                await redisDatabase.HashDeleteAsync(hashKey, hashFields.Select(k => (RedisValue)k).ToArray());
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(DeleteHashByKeysAsync)}: {hashKey}{string.Join(", ", hashFields.ToArray())}");
            }
        }

        public async Task DeleteHashByKeysAsync(string hashKey, List<string> hashFields, int database)
        {
            var databaseNumber = database == -1 ? _options.Database : database;

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);
                await redisDatabase.HashDeleteAsync(hashKey, hashFields.Select(k => (RedisValue)k).ToArray());
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(DeleteHashByKeysAsync)} from db {databaseNumber}: {hashKey}{string.Join(", ", hashFields.ToArray())}");
            }
        }

        public async Task<bool> ClearHashAsync(string hashKey)
        {
            LogInformation($"Start Redis {nameof(ClearHashAsync)}: {hashKey}");

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase();
                bool result = await redisDatabase.KeyDeleteAsync(hashKey);
                LogInformation($"End Redis {nameof(ClearHashAsync)}: {hashKey}");
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(ClearHashAsync)}: {hashKey}");
                return false;
            }
        }

        public async Task<bool> ClearHashAsync(string hashKey, int database)
        {
            var databaseNumber = database == -1 ? _options.Database : database;
            LogInformation($"Start Redis {nameof(ClearHashAsync)} from db {databaseNumber}: {hashKey}");

            try
            {
                IDatabase redisDatabase = Connection.GetDatabase(databaseNumber);
                bool result = await redisDatabase.KeyDeleteAsync(hashKey);
                LogInformation($"End Redis {nameof(ClearHashAsync)} from db {databaseNumber}: {hashKey}");
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error in {nameof(ClearHashAsync)} from db {databaseNumber}: {hashKey}");
                return false;
            }
        }

        private void LogInformation(string message)
        {
            _logger.LogInformation($"RedisCache Message: {message}");
        }

        private void LogError(Exception exception, string message)
        {
            _logger.LogError(exception, $"RedisCache Message: {message} | {exception.Message}");
        }
    }
}
