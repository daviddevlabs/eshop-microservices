using Ordering.Application.Common;
using StackExchange.Redis;

namespace Ordering.Infrastructure.Caching;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<string?> GetAsync(string key)
    {
        return await _database.StringGetAsync(key);
    }

    public async Task<Dictionary<string, string?>> GetMultipleAsync(IEnumerable<string> keys)
    {
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        var values = await _database.StringGetAsync(redisKeys);

        return redisKeys.Zip(values, (key, value) => new { key, value })
            .ToDictionary(
                kv => kv.key.ToString(),
                kv => kv.value.HasValue ? kv.value.ToString() : null);
    }

    public async Task SetAsync(string key, string value, TimeSpan expiration)
    {
        await _database.StringSetAsync(key, value, expiration);
    }
}