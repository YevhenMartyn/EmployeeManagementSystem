
using BusinessLogicLayer.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BusinessLogicLayer.Services
{
    public class CacheService<T> : ICacheService<T> where T : class
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService<T>> _logger;
        private readonly string _cacheKeyPrefix;
        public CacheService(IDistributedCache cache, ILogger<CacheService<T>> logger)
        {
            _cache = cache;
            _logger = logger;
            _cacheKeyPrefix = $"{typeof(T).Name}_";
        }

        public async Task<T> GetCacheAsync<T>(string cacheKey)
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(cachedData))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetCacheAsync<T>(string cacheKey, T data)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // Cache expiration time
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), options);
        }

        public async Task InvalidateCacheAsync(int id)
        {
            await _cache.RemoveAsync($"{_cacheKeyPrefix}{id}");
            await _cache.RemoveAsync($"{_cacheKeyPrefix}All");
        }

        public async Task<string> GetCacheKeyPrefixAsync() { 
            return _cacheKeyPrefix;
        }
    }
}
