namespace RentACar.Application.Abstracts.Services;

public interface IRedisService
{
    
    Task AddToBlacklistAsync(string token, TimeSpan expiry);
    Task<bool> IsTokenBlacklistedAsync(string token);
}
