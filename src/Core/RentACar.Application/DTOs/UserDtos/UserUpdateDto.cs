namespace RentACar.Application.DTOs.UserDtos;

public class UserUpdateDto
{
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool IsEmailConfirmed { get; set; }
}

