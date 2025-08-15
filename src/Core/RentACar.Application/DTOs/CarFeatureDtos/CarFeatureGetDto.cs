namespace RentACar.Application.DTOs.CarFeatureDto;

public class CarFeatureGetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

