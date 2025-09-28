using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Shared.Helpers;
using RentACar.Application.Shared;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.RoleDtos;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/<RolesController>
    //[Authorize(Policy = Permissions.Role.GetAllPermission)]
    [HttpGet("permissions")]
    public IActionResult GetAllPermissions()
    {
        var permissions = PermissionHelper.GetAllPermissions();
        return Ok(permissions);
    }

    
    [HttpPost("Create Role")]
    //[Authorize(Policy = Permissions.Role.Create)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.PartialContent)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create(RoleCreateDto dto)
    {
        var result = await _roleService.CreateRole(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    
    [HttpPut("{id}")]
    //[Authorize(Policy = Permissions.Role.Update)]
    [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody] RoleUpdateDto dto)
    {
        if (id != dto.Id.ToString())
            return BadRequest("ID mismatch");

        var result = await _roleService.UpdateRole(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    
    [HttpDelete("{roleName}")]
    //[Authorize(Policy = Permissions.Role.Delete)]
    [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete(string roleName)
    {
        var result = await _roleService.DeleteRole(roleName);
        return StatusCode((int)result.StatusCode, result);
    }

    
    [HttpGet]
    //[Authorize(Policy = Permissions.Role.GetAllRoles)]
    [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
    public IActionResult GetAllRoles()
    {
        var roles = _roleService.GetAllRoles();
        return Ok(roles);
    }


}
