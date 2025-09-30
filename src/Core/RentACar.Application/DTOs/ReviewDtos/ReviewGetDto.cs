namespace RentACar.Application.DTOs.ReviewDtos;

public class ReviewGetDto
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public string CarBrand { get; set; } = null!;
    public string CarModel { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public byte Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

