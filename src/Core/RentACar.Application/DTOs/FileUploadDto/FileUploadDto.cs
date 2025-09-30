using Microsoft.AspNetCore.Http;

namespace RentACar.Application.DTOs.FileUploadDto;

public class FileUploadDto
{
    public IFormFile File { get; set; } = null!;
}