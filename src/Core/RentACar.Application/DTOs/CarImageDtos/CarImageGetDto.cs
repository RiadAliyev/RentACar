namespace RentACar.Application.DTOs.CarImageDtos;

public class CarImageGetDto
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<CarImageGetDto> Images { get; set; } = new();
}
