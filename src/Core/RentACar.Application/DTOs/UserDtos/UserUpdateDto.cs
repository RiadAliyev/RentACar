namespace RentACar.Application.DTOs.UserDtos;

public sealed class UserUpdateDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Domain.Enums.AccountType? AccountType { get; set; }
}
