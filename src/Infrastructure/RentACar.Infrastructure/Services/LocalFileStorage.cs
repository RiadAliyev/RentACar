using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RentACar.Application.Abstracts.Services;
using System.Linq;

namespace RentACar.Infrastructure.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _env;
    public LocalFileStorage(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveCarImageAsync(IFormFile file, Guid carId)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Empty file.");

        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("Max 5MB.");

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
            throw new InvalidOperationException("Invalid file type.");

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var root = Path.Combine(webRoot, "uploads", "cars", carId.ToString());
        Directory.CreateDirectory(root);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(root, fileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
            await file.CopyToAsync(stream);

        return $"/uploads/cars/{carId}/{fileName}";
    }

    public void DeleteIfExists(string? relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl)) return;

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var path = Path.Combine(webRoot, relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(path)) File.Delete(path);
    }
}
