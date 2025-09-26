using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.UserDtos;
using RentACar.Application.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

    //[Authorize(Policy = Permissions.Account.AddRole)]
    [HttpPost("Assign-roles")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddRole([FromBody] UserAddRoleDto dto)
    {
        var result = await _userService.AddRole(dto);
        return StatusCode((int)result.StatusCode, result);
    }




}
