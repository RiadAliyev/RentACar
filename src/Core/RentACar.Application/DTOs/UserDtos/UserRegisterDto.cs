using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.UserDtos;

public class UserRegisterDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public AccountType AccountType { get; set; } = AccountType.Customer;

    public string? CompanyName { get; set; }
    public string? CompanyRegistrationNumber { get; set; }
    public string? CompanyAddress { get; set; }

}
