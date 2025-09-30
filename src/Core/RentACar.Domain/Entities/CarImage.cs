namespace RentACar.Domain.Entities;

public class CarImage: BaseEntity
{
    public Guid CarId { get; set; }
    public string ImageUrl { get; set; } = null!;

    public Car Car { get; set; } = null!;
}

