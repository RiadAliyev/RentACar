namespace RentACar.Domain.Entities;

public class Review: BaseEntity
{
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public byte Rating { get; set; }
    public string Comment { get; set; } = null!;

    public Car Car { get; set; } = null!;
    public User Customer { get; set; } = null!;
}

