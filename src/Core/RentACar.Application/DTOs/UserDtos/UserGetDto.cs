namespace RentACar.Application.DTOs.UserDtos;

public class UserGetDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }

    // Əlavə məlumat
    public string? CompanyName { get; set; }
    public List<string> CarBrands { get; set; } = new();
}

