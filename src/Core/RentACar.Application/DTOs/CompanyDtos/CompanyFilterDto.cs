namespace RentACar.Application.DTOs.CompanyDtos;

public class CompanyFilterDto
{
    public string? Name { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Address { get; set; }
    public Guid? OwnerId { get; set; }
}
