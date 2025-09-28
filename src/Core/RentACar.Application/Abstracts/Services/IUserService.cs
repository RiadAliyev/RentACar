using RentACar.Application.DTOs.UserDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface IUserService
{
    Task<BaseResponse<string>> Register(UserRegisterDto dto);
    Task<BaseResponse<TokenResponse>> Login(UserLoginDto dto);
    Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<BaseResponse<string>> AddRole(UserAddRoleDto dto);
    Task<BaseResponse<List<UserListItemDto>>> GetAllUsersAsync(UserFilterDto filter);
    Task<BaseResponse<UserDetailsDto>> GetUserByIdAsync(Guid id);
    Task<BaseResponse<string>> ResetPassword(ResetPasswordDto dto);
    Task<BaseResponse<MeProfileDto>> GetMeAsync();
    Task<BaseResponse<string>> ConfirmEmail(string userId, string token);
}
