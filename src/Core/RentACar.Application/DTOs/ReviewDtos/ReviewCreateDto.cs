namespace RentACar.Application.DTOs.ReviewDtos;

public class ReviewCreateDto
{
    public Guid CarId { get; set; }
    public Guid CustomerId { get; set; }
    public byte Rating { get; set; }
    public string Comment { get; set; } = null!;
}

