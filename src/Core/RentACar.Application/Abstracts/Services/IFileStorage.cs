using Microsoft.AspNetCore.Http;

namespace RentACar.Application.Abstracts.Services;

public interface IFileStorage
{
    Task<string> SaveCarImageAsync(IFormFile file, Guid carId);
    void DeleteIfExists(string? relativeUrl);
}
