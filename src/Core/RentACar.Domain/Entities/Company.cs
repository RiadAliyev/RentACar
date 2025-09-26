namespace RentACar.Domain.Entities;

public class Company: BaseEntity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public string Address { get; set; } = null!;

    public AppUser Owner { get; set; } = null!;
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}

