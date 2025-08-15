namespace RentACar.Application.DTOs.ReviewDtos;

public class ReviewGetDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string CarBrand { get; set; } = null!;
    public string CarModel { get; set; } = null!;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public byte Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

