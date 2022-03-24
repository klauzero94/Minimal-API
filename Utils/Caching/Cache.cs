using Settings;
using StackExchange.Redis;

namespace Utils.Caching;

public class Cache
{
    private IConnectionMultiplexer Connection;
    public Cache() => Connection = ConnectionMultiplexer.Connect(CacheSettings.ConnectionString);
    public async Task<bool> CheckIfCacheExistsAsync(string key)
    {
        var db = Connection.GetDatabase();
        return await db.KeyExistsAsync(key);
    }

    public async Task<string> GetCacheValueAsync(string key)
    {
        var db = Connection.GetDatabase();
        return await db.StringGetAsync(key);
    }

    public async Task RemoveCacheValueAsync(string key)
    {
        var db = Connection.GetDatabase();
        await db.KeyDeleteAsync(key);
    }

    public async Task SetCacheValueAsync(string key, string value)
    {
        var db = Connection.GetDatabase();
        await db.StringSetAsync(key, value);
    }

    public async Task SetCacheValueAsync(string key, string value, double expiration)
    {
        var db = Connection.GetDatabase();
        await db.StringSetAsync(key, value);
        await db.KeyExpireAsync(key, TimeSpan.FromSeconds(expiration));
    }
}