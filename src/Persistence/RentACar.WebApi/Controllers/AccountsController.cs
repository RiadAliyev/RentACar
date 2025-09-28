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

    [HttpGet]
    [Authorize(Policy = Permissions.Account.GetAll)]
    [ProducesResponseType(typeof(BaseResponse<List<UserListItemDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
    {
        var res = await _userService.GetAllUsersAsync(filter);
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

}
