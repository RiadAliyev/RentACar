namespace RentACar.Domain.Entities;

public class CarFeature: BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<CarFeatureAssignment> Cars { get; set; } = new List<CarFeatureAssignment>();
}

