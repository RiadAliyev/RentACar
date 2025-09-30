namespace RentACar.Application.DTOs.UserDtos;

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
