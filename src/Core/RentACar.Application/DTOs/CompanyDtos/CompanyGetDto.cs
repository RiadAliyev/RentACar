namespace RentACar.Application.DTOs.CompanyDtos;

public class CompanyGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<string> CarNames { get; set; } = new();
}

