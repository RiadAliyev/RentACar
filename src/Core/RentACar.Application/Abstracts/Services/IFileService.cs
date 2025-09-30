using Microsoft.AspNetCore.Http;

namespace RentACar.Application.Abstracts.Services;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
    Task<bool> DeleteAsync(string fileUrl);
}
