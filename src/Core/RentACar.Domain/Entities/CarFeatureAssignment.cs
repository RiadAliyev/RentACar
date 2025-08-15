namespace RentACar.Domain.Entities;

public class CarFeatureAssignment: BaseEntity
{
    public int CarId { get; set; }
    public int FeatureId { get; set; }

    public Car Car { get; set; } = null!;
    public CarFeature Feature { get; set; } = null!;
}

