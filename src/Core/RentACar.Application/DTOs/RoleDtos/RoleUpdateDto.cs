namespace RentACar.Application.DTOs.RoleDtos;

public class RoleUpdateDto
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = null!;
    public List<string> PermissionList { get; set; } = new();
}
