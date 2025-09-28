using Microsoft.AspNetCore.Http;

namespace RentACar.Application.DTOs.CarImageDtos;

public class CarImageReplaceDto
{
    public IFormFile File { get; set; } = default!;
}
