using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.UserDtos;
using RentACar.Application.Shared;
using RentACar.Application.Shared.Settings;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Services;

public class UserService:IUserService
{
    private UserManager<AppUser> _userManager {  get; }
    private SignInManager<AppUser> _singInManager { get; }
    private JWTSettings _jwtSetting { get; }

    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public UserService(UserManager<AppUser> userManager,
        SignInManager<AppUser> singInManager,
        IOptions<JWTSettings> jwtSetting ,
        RoleManager<IdentityRole<Guid>> rolemanager)
    {
        _userManager = userManager;
        _singInManager = singInManager;
        _jwtSetting = jwtSetting.Value;
        _roleManager = rolemanager;
    }

    public async Task<BaseResponse<string>> Register(UserRegisterDto dto)
    {
        var existedEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existedEmail is not null)
        {
            return new BaseResponse<string>("This account already exist", HttpStatusCode.BadRequest);
        }
        AppUser newUser = new()
        {
            Email = dto.Email,
            FullName = dto.FullName,
            UserName= dto.Email
        };

        IdentityResult identityResult = await _userManager.CreateAsync(newUser, dto.Password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors;
            StringBuilder errorMassege = new();
            foreach (var error in errors)
            {
                errorMassege.Append(error.Description + ";");
            }
            return new(errorMassege.ToString(), HttpStatusCode.BadRequest);
        }

        return new("Successfully created", true, HttpStatusCode.Created);
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

    private async Task<TokenResponse> GenerateTokensAsync(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
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
}
