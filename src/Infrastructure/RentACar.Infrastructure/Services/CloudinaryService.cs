using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.Shared.Settings;
using Microsoft.Extensions.Options;

namespace RentACar.Infrastructure.Services;

public class CloudinaryService : IFileService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "rentacar" // Cloudinary-də qovluq adı
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception(uploadResult.Error.Message);

        return uploadResult.SecureUrl.ToString();
    }

    public async Task<bool> DeleteAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return false;

        // PublicId çıxarmaq (Cloudinary-də faylın unikal adı)
        var segments = new Uri(fileUrl).Segments;
        var fileName = segments.Last();
        var publicId = Path.GetFileNameWithoutExtension(fileName);

        var deletionParams = new DeletionParams($"rentacar/{publicId}");

        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Result == "ok";
    }
}