namespace RentACar.Domain.Entities;

public class Review: BaseEntity
{
    public Guid CarId { get; set; }
    public Guid CustomerId { get; set; }
    public byte Rating { get; set; }
    public string Comment { get; set; } = null!;

    public Car Car { get; set; } = null!;
    public User Customer { get; set; } = null!;
}

