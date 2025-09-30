namespace RentACar.Domain.Entities;

public class CarFeatureAssignment: BaseEntity
{
    public Guid CarId { get; set; }
    public Guid FeatureId { get; set; }

    public Car Car { get; set; } = null!;
    public CarFeature Feature { get; set; } = null!;
}

