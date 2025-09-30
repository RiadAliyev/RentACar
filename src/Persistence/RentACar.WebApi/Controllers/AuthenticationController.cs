using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.UserDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private IUserService _userService { get; }
    public AuthenticationController(IUserService userService)
    {
        _userService = userService;
    }

    
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await _userService.Register(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await _userService.Login(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var result = await _userService.ConfirmEmail(userId, token);
        if (!result.Success)
            return BadRequest(result);
        return StatusCode((int)result.StatusCode, result);
    }

    
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmDto dto)
    {
        var result = await _userService.ResendConfirmationEmailAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize] 
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddOrUpdateNumber([FromBody] AddNumberDto dto)
    {
        var result = await _userService.AddOrUpdateNumberAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest dto)
    {
        var result = await _userService.RefreshTokenAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _userService.ChangePasswordAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var result = await _userService.ForgotPasswordAsync(dto);
        return StatusCode((int)result.StatusCode, result);

    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _userService.ResetPassword(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
    [Authorize] 
    [ProducesResponseType(typeof(BaseResponse<MeProfileDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<MeProfileDto>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<MeProfileDto>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UserAbout()
    {
        var res = await _userService.GetMeAsync();
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost]
    [Authorize] 
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Logout()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var result = await _userService.Logout(token);
        return StatusCode((int)result.StatusCode, result);
    }

}
