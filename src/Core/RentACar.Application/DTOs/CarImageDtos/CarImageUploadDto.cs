using Microsoft.AspNetCore.Http;

namespace RentACar.Application.DTOs.CarImageDtos;

public class CarImageUploadDto
{
    public List<IFormFile> Files { get; set; } = new();
}

