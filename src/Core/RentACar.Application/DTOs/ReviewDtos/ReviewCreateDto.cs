namespace RentACar.Application.DTOs.ReviewDtos;

public class ReviewCreateDto
{
    public Guid CarId { get; set; }
    public byte Rating { get; set; }   // 1 5 arasi yaz
    public string Comment { get; set; } = null!;
}

