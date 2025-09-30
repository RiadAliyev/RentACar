using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.UserDtos;
using RentACar.Application.Shared;
using RentACar.Application.Shared.Settings;
using RentACar.Domain.Entities;
using RentACar.Domain.Enums;
using RentACar.Infrastructure.Services;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Services;

public class UserService:IUserService
{
    private readonly RentACarDbContext _db;
    private UserManager<AppUser> _userManager {  get; }
    private SignInManager<AppUser> _singInManager { get; }

    private IEmailService _emailService { get; }
    private JWTSettings _jwtSetting { get; }

    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    private readonly IHttpContextAccessor _http;
    private readonly IRedisService _redisService;

    public UserService(UserManager<AppUser> userManager,
        SignInManager<AppUser> singInManager,
        IOptions<JWTSettings> jwtSetting ,
        RoleManager<IdentityRole<Guid>> rolemanager,
        RentACarDbContext db,
        IHttpContextAccessor httpContextAccessor,
        IEmailService emailService,
        IRedisService redisService)
    {
        _userManager = userManager;
        _singInManager = singInManager;
        _jwtSetting = jwtSetting.Value;
        _roleManager = rolemanager;
        _db = db;
        _http = httpContextAccessor;
        _emailService = emailService;
        _redisService = redisService;
    }

    public async Task<BaseResponse<string>> Register(UserRegisterDto dto)
    {
        // 1) Email unik olmalıdır
        var existedEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existedEmail is not null)
            return new BaseResponse<string>("This account already exists", HttpStatusCode.BadRequest);

        // 2) İstifadəçi obyektini hazırla
        var newUser = new AppUser
        {
            Email = dto.Email,
            FullName = dto.FullName,
            UserName = dto.Email,
            AccountType = dto.AccountType
        };

        // 3) Identity-də yarat
        var identityResult = await _userManager.CreateAsync(newUser, dto.Password);
        await _userManager.AddClaimAsync(newUser, new Claim("account_type", newUser.AccountType.ToString()));

        if (!identityResult.Succeeded)
        {
            var errorMessage = string.Join("; ", identityResult.Errors.Select(e => e.Description));
            return new BaseResponse<string>(errorMessage, HttpStatusCode.BadRequest);
        }

        // 4) Rol təyin et
        var roleName = dto.AccountType switch
        {
            AccountType.CompanyOwner => "CompanyOwner",
            AccountType.CarOwner => "CarOwner",
            _ => "Customer"
        };
        if (await _roleManager.RoleExistsAsync(roleName))
            await _userManager.AddToRoleAsync(newUser, roleName);

        // 5) Email təsdiqi üçün link göndər
        string confirmEmailLink = await GetEmailConfirmLink(newUser);
        await _emailService.SendEmailAsync(new List<string> { newUser.Email }, "Email Confirmation", confirmEmailLink);

        return new BaseResponse<string>("Successfully registered", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<TokenResponse>> Login(UserLoginDto dto)
    {
        var existedUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existedUser is null)
        {
            return new("Email or password os wrong.", HttpStatusCode.NotFound);
        }

        if (!existedUser.EmailConfirmed)
        {
            return new("Please Confirm your email", HttpStatusCode.BadRequest);
        }

        SignInResult signInResult = await _singInManager.PasswordSignInAsync
            (dto.Email, dto.Password, true, true);

        if (signInResult.IsLockedOut)
        {
            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(existedUser);
            return new($"Your account is locked until {lockoutEnd?.UtcDateTime}.", null, HttpStatusCode.Forbidden);
        }

        if (signInResult.IsNotAllowed)
        {
            return new("You are not allowed to login. Please check your email confirmation.", null, HttpStatusCode.Forbidden);
        }

        if (!signInResult.Succeeded)
        {
            return new("Email or password os wrong.", null, HttpStatusCode.NotFound);
        }
        var token = await GenerateTokensAsync(existedUser);

        return new("Token generated", token, HttpStatusCode.OK);



    }

    public async Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return new("Invalid access token", null, HttpStatusCode.BadRequest);

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
            return new("User not faund", null, HttpStatusCode.NotFound);



        if (user.RefreshToken is null || user.RefreshToken != request.RefreshToken ||
            user.ExpireDate < DateTime.UtcNow)
            return new("Invalid refresh token", null, HttpStatusCode.BadRequest);


        //Generate new tokens
        var tokenResponse = await GenerateTokensAsync(user);
        return new("Refreshed", tokenResponse, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new BaseResponse<string>("User not found", null, HttpStatusCode.NotFound);


        var decodedToken = HttpUtility.UrlDecode(token);
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
            return new BaseResponse<string>($"Email confirmation failed: {errorMessages}", null, HttpStatusCode.BadRequest);
        }

        return new BaseResponse<string>("Email confirmed successfully", null, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> AddRole(UserAddRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user is null)
        {
            return new BaseResponse<string>("User not found", HttpStatusCode.NotFound);
        }

        var roleNames = new List<string>();

        foreach (var roleId in dto.RoleId.Distinct())
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null)
            {
                return new BaseResponse<string>($"Role With Id'{roleId}' not found", HttpStatusCode.NotFound);
            }

            if (!await _userManager.IsInRoleAsync(user, role.Name!))
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new BaseResponse<string>($"Failed to add role '{role.Name}'to user:{errors}", HttpStatusCode.BadRequest);
                }
                roleNames.Add(role.Name!);
            }

        }
        return new BaseResponse<string>($"Succesfuly added roles:{string.Join(", ", roleNames)}", HttpStatusCode.OK);

    }

    public async Task<BaseResponse<List<UserListItemDto>>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync();

        var result = new List<UserListItemDto>(users.Count);
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            result.Add(new UserListItemDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email!,
                EmailConfirmed = u.EmailConfirmed,
                AccountType = u.AccountType,
                Roles = roles.ToList()
            });
        }

        return new BaseResponse<List<UserListItemDto>>(
            message: $"Users listed. total={result.Count}",
            data: result,
            statusCode: HttpStatusCode.OK
        );
    }

    public async Task<BaseResponse<UserDetailsDto>> GetUserByIdAsync(Guid id)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return new BaseResponse<UserDetailsDto>("User not found", HttpStatusCode.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new UserDetailsDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles.ToList()
        };

        return new BaseResponse<UserDetailsDto>("User fetched", dto, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return new("User not found", HttpStatusCode.NotFound);
        var decodedToken = Uri.UnescapeDataString(dto.Token);
        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
            return new(errorMessage, HttpStatusCode.BadRequest);
        }

        return new("Password reset successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<MeProfileDto>> GetMeAsync()
    {
        var userIdStr = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
            return new BaseResponse<MeProfileDto>("Unauthorized", HttpStatusCode.Unauthorized);

        var userId = Guid.Parse(userIdStr);

        // Eager-load: Owner üçün u.Cars; Company üçün u.Company.Cars
        var user = await _userManager.Users
            .Include(u => u.Cars)
            .Include(u => u.Company)!.ThenInclude(c => c.Cars)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return new BaseResponse<MeProfileDto>("User not found", HttpStatusCode.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        // Rollardan Permission claim-ləri topla (varsınızsa)
        var permissions = new List<string>();
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) continue;
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            permissions.AddRange(roleClaims.Where(c => c.Type == "Permission").Select(c => c.Value));
        }
        permissions = permissions.Distinct().OrderBy(p => p).ToList();

        var dto = new MeProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName!,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            AccountType = user.AccountType,
            Roles = roles.ToList(),
            
        };

        // AccountType qaydası:
        if (user.AccountType == AccountType.CarOwner)
        {
            var cars = (user.Cars ?? new List<Car>())
                .Select(c => new CarInfoDto
                {
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    RentPrice = c.DailyPrice
                })
                .ToList();

            dto.Cars = cars;
            dto.CarCount = cars.Count;
        }
        else if (user.AccountType == AccountType.CompanyOwner && user.Company is not null)
        {
            var cars = (user.Company.Cars ?? new List<Car>())
                .Select(c => new CarInfoDto
                {
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    RentPrice = c.DailyPrice
                })
                .ToList();

            dto.Cars = cars;
            dto.CarCount = cars.Count;
        }
        // Buyer üçün Cars boş qalır

        return new BaseResponse<MeProfileDto>("Profile fetched", dto, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return new("User not found", HttpStatusCode.NotFound);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var link = $"https://localhost:7017/api/Authentication/ResetPassword?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

        await _emailService.SendEmailAsync(
            new List<string> { user.Email! },
            "Reset Password",
            $"Click here to reset your password: {link}");

        return new("Reset password link sent to email", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> ResendConfirmationEmailAsync(ResendConfirmDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return new("User not found", HttpStatusCode.NotFound);

        if (user.EmailConfirmed)
            return new("Email already confirmed", HttpStatusCode.BadRequest);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = $"https://localhost:7017/api/Authentication/ConfirmEmail?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

        await _emailService.SendEmailAsync(
            new List<string> { user.Email! },
            "Confirm your email",
            $"Click this link to confirm your account: {link}");

        return new("Confirmation email resent", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var userId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return new("Unauthorized", HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new("User not found", HttpStatusCode.NotFound);

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new($"Failed to change password: {errors}", HttpStatusCode.BadRequest);
        }

        return new("Password changed successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> Logout(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Qalan vaxtı TimeSpan kimi hesabla
        var expireAt = jwtToken.ValidTo.ToUniversalTime() - DateTime.UtcNow;

        if (expireAt <= TimeSpan.Zero)
            expireAt = TimeSpan.FromMinutes(1); // Ən azı 1 dəqiqə saxla ki, Redisə düşsün

        await _redisService.AddToBlacklistAsync(token, expireAt);

        return new BaseResponse<string>("Logged out successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<UserDetailsDto>> UpdateUserAsync(Guid id, UserUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return BaseResponse<UserDetailsDto>.FailResponse("User not found", HttpStatusCode.NotFound);

        if (!string.IsNullOrWhiteSpace(dto.FullName))
            user.FullName = dto.FullName;
        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;
        if (dto.AccountType.HasValue)
            user.AccountType = dto.AccountType.Value;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BaseResponse<UserDetailsDto>.FailResponse(errors, HttpStatusCode.BadRequest);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var dtoRes = new UserDetailsDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles.ToList()
        };

        return BaseResponse<UserDetailsDto>.SuccessResponse(dtoRes, "User updated successfully");
    }

    public async Task<BaseResponse<bool>> DeleteUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return BaseResponse<bool>.FailResponse("User not found", HttpStatusCode.NotFound);

        // Soft delete: lock user instead of removing
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;
        await _userManager.UpdateAsync(user);

        return BaseResponse<bool>.SuccessResponse(true, "User deactivated successfully");
    }

    public async Task<BaseResponse<bool>> LockUserAsync(Guid id, DateTimeOffset lockUntil)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return BaseResponse<bool>.FailResponse("User not found", HttpStatusCode.NotFound);

        user.LockoutEnabled = true;
        user.LockoutEnd = lockUntil;
        await _userManager.UpdateAsync(user);

        return BaseResponse<bool>.SuccessResponse(true, $"User locked until {lockUntil}");
    }

    public async Task<BaseResponse<bool>> UnlockUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return BaseResponse<bool>.FailResponse("User not found", HttpStatusCode.NotFound);

        user.LockoutEnd = null;
        user.LockoutEnabled = false;
        await _userManager.UpdateAsync(user);

        return BaseResponse<bool>.SuccessResponse(true, "User unlocked successfully");
    }

    public async Task<BaseResponse<bool>> AdminResetPasswordAsync(Guid id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return BaseResponse<bool>.FailResponse("User not found", HttpStatusCode.NotFound);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BaseResponse<bool>.FailResponse($"Failed to reset password: {errors}", HttpStatusCode.BadRequest);
        }

        return BaseResponse<bool>.SuccessResponse(true, "Password reset successfully");
    }

    public async Task<BaseResponse<List<UserRoleDto>>> GetUsersByRoleAsync(string roleName)
    {
        var users = await _userManager.GetUsersInRoleAsync(roleName);
        if (users == null || users.Count == 0)
            return BaseResponse<List<UserRoleDto>>.FailResponse("No users found in this role", HttpStatusCode.NotFound);

        var result = new List<UserRoleDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserRoleDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList(),
                AccountType = user.AccountType
            });
        }

        return BaseResponse<List<UserRoleDto>>.SuccessResponse(result, "Users fetched successfully");
    }

    public async Task<BaseResponse<string>> AddOrUpdateNumberAsync(AddNumberDto dto)
    {
        var userIdStr = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
            return new BaseResponse<string>("Unauthorized", HttpStatusCode.Unauthorized);

        var userId = Guid.Parse(userIdStr);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return new BaseResponse<string>("User not found", HttpStatusCode.NotFound);

        // Telefon nömrəsini update et
        user.PhoneNumber = dto.Number;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new BaseResponse<string>($"Failed to update number: {errors}", HttpStatusCode.BadRequest);
        }

        return new BaseResponse<string>("Phone number updated successfully", HttpStatusCode.OK);
    }

    private async Task<TokenResponse> GenerateTokensAsync(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("account_type", user.AccountType.ToString()),   // <<-- ƏLAVƏ
            new Claim("full_name", user.FullName ?? string.Empty)
        };
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                var permissionClaims = roleClaims.Where(c => c.Type == "Permission").Distinct();

                foreach (var permissionClaim in permissionClaims)
                {
                    claims.Add(new Claim("Permission", permissionClaim.Value));
                }
            }
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.ExpiryMinutes),
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        //create refresh token
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryDate = DateTime.UtcNow.AddHours(2);
        user.RefreshToken = refreshToken;
        user.ExpireDate = refreshTokenExpiryDate;
        await _userManager.UpdateAsync(user);

        return new TokenResponse
        {
            Token = jwt,
            RefreshToken = refreshToken,
            ExpireDate = tokenDescriptor.Expires!.Value,
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng =RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // expired token üçün bu false olmalıdır

            ValidIssuer = _jwtSetting.Issuer,
            ValidAudience = _jwtSetting.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    private async Task<string> GetEmailConfirmLink(AppUser user)
    {
        //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //var link = $"https://localhost:7017/api/Accounts/ConfirmEmail?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
        //Console.WriteLine("Confirm Email Link" + link);
        //return link;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Tokeni URL üçün encode edirik
        var encodedToken = HttpUtility.UrlEncode(token);

        // Burada doğru controller və endpoint göstərilməlidir

        var link = $"https://localhost:7017/api/Authentication/ConfirmEmail?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

        return link;
    }
}
