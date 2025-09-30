namespace RentACar.Application.DTOs.UserDtos;

public sealed class UserRoleDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
    public Domain.Enums.AccountType AccountType { get; set; }
}
