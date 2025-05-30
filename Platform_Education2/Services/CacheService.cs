﻿using Microsoft.Extensions.Caching.Distributed;
using PlatformEduPro.Services.Interfaces;
using System.Text.Json;

namespace PlatformEduPro.Services
{

    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;
       

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
          

            var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

            return cachedValue is null
                ? null
                : JsonSerializer.Deserialize<T>(cachedValue);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
           

            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {

            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}
