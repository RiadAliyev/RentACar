namespace RentACar.Application.DTOs.UserDtos;

public class ResetPasswordDto
{
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
