using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.UserDtos;
using RentACar.Application.Shared;


namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private IUserService _userService { get; }
    public AccountsController(IUserService userService)
    {
        _userService=userService;   
    }

    [Authorize(Policy = Permissions.Account.AddRole)]
    [HttpPost("Assign-roles")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddRole([FromBody] UserAddRoleDto dto)
    {
        var result = await _userService.AddRole(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("GetAllUsers")]
    [Authorize(Policy = Permissions.Account.GetAllUsers)]
    [ProducesResponseType(typeof(BaseResponse<List<UserListItemDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        var res = await _userService.GetAllUsersAsync();
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permissions.Account.GetById)]
    [ProducesResponseType(typeof(BaseResponse<UserDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<UserDetailsDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var res = await _userService.GetUserByIdAsync(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.Account.Update)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto dto)
    {
        var res = await _userService.UpdateUserAsync(id, dto);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Permissions.Account.Delete)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var res = await _userService.DeleteUserAsync(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("{id:guid}/lock")]
    [Authorize(Policy = Permissions.Account.Lock)]
    public async Task<IActionResult> LockUser(Guid id, [FromQuery] DateTimeOffset? endDate)
    {
        var res = await _userService.LockUserAsync(id, endDate ?? DateTimeOffset.UtcNow.AddDays(7));
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("{id:guid}/unlock")]
    [Authorize(Policy = Permissions.Account.Unlock)]
    public async Task<IActionResult> UnlockUser(Guid id)
    {
        var res = await _userService.UnlockUserAsync(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("{id:guid}/reset-password")]
    [Authorize(Policy = Permissions.Account.ResetPassword)]
    public async Task<IActionResult> ResetUserPassword(Guid id, [FromBody] AdminResetPasswordDto dto)
    {
        var res = await _userService.AdminResetPasswordAsync(id, dto.NewPassword);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("by-role/{roleName}")]
    [Authorize(Policy = Permissions.Account.GetByRole)]
    public async Task<IActionResult> GetUsersByRole(string roleName)
    {
        var res = await _userService.GetUsersByRoleAsync(roleName);
        return StatusCode((int)res.StatusCode, res);
    }

}
