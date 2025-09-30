using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.UserDtos;

public sealed class UserListItemDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
    public AccountType AccountType { get; set; }
}
