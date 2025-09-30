using RentACar.Application.DTOs.UserDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface IUserService
{
    Task<BaseResponse<string>> Register(UserRegisterDto dto);
    Task<BaseResponse<TokenResponse>> Login(UserLoginDto dto);
    Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<BaseResponse<string>> AddRole(UserAddRoleDto dto);
    Task<BaseResponse<List<UserListItemDto>>> GetAllUsersAsync();
    Task<BaseResponse<UserDetailsDto>> GetUserByIdAsync(Guid id);
    Task<BaseResponse<string>> ResetPassword(ResetPasswordDto dto);
    Task<BaseResponse<MeProfileDto>> GetMeAsync();
    Task<BaseResponse<string>> ConfirmEmail(string userId, string token);
    Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<BaseResponse<string>> ChangePasswordAsync(ChangePasswordDto dto);
    Task<BaseResponse<string>> ResendConfirmationEmailAsync(ResendConfirmDto dto);
    Task<BaseResponse<string>> Logout(string token);
    Task<BaseResponse<UserDetailsDto>> UpdateUserAsync(Guid id, UserUpdateDto dto);
    Task<BaseResponse<bool>> DeleteUserAsync(Guid id);
    Task<BaseResponse<bool>> LockUserAsync(Guid id, DateTimeOffset lockUntil);
    Task<BaseResponse<bool>> UnlockUserAsync(Guid id);
    Task<BaseResponse<bool>> AdminResetPasswordAsync(Guid id, string newPassword);
    Task<BaseResponse<List<UserRoleDto>>> GetUsersByRoleAsync(string roleName);
    Task<BaseResponse<string>> AddOrUpdateNumberAsync(AddNumberDto dto);
}
