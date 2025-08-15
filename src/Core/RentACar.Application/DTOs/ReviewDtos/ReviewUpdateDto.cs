namespace RentACar.Application.DTOs.ReviewDtos;

public class ReviewUpdateDto
{
    public byte Rating { get; set; }
    public string Comment { get; set; } = null!;
}

