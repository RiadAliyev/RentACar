namespace RentACar.Application.Abstracts.Services;

public interface IRedisService
{
    

    // 🔑 Blacklist üçün əlavə metodlar
    Task AddToBlacklistAsync(string token, TimeSpan expiry);
    Task<bool> IsTokenBlacklistedAsync(string token);
}
