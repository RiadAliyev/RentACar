using Microsoft.EntityFrameworkCore.Storage;
using RentACar.Application.Abstracts.Services;
using StackExchange.Redis;

namespace RentACar.Infrastructure.Services;

public class RedisService : IRedisService
{
    
    private readonly StackExchange.Redis.IDatabase _db;

    


    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task AddToBlacklistAsync(string token, TimeSpan expiry)
    {
        await _db.StringSetAsync($"blacklist:{token}", true, expiry);
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _db.KeyExistsAsync($"blacklist:{token}");
    }
}
