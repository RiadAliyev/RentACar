namespace RentACar.Application.DTOs.CarImageDtos;

public class CarImageGetDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
