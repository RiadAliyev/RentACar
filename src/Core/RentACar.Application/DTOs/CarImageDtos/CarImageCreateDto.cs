namespace RentACar.Application.DTOs.CarImageDtos;

public class CarImageCreateDto
{
    public Guid CarId { get; set; }
    public string ImageUrl { get; set; } = null!;
}

