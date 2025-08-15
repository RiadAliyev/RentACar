namespace RentACar.Domain.Entities;

public class Bio: BaseEntity
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? ValueType { get; set; }

}

