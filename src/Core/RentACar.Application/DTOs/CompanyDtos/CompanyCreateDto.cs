namespace RentACar.Application.DTOs.CompanyDtos;

public class CompanyCreateDto
{
    public int OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
}

