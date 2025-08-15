namespace RentACar.Application.DTOs.ReviewDtos;

public class ReviewCreateDto
{
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public byte Rating { get; set; }
    public string Comment { get; set; } = null!;
}

