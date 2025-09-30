using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CompanyDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _companyService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }


    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _companyService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpPost]
    [Authorize(Roles = "CompanyOwner")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] CompanyCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("validation error", HttpStatusCode.BadRequest));

        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(ownerIdStr))
            return Unauthorized(BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized));

        var ownerId = Guid.Parse(ownerIdStr);

        var response = await _companyService.CreateAsync(dto, ownerId); 
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CompanyOwnerOnly")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CompanyUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("validation error", HttpStatusCode.BadRequest));

        
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(ownerIdStr))
            return Unauthorized(BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized));
        var ownerId = Guid.Parse(ownerIdStr);

        
        var existing = await _companyService.GetByIdAsync(id);
        if (!existing.Success || existing.Data is null)
            return StatusCode((int)existing.StatusCode, existing);

        if (existing.Data.OwnerId != ownerId)
            return Forbid(); 

       
        var response = await _companyService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CompanyOwnerOnly")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Delete(Guid id)
    {
        
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(ownerIdStr))
            return Unauthorized(BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized));
        var ownerId = Guid.Parse(ownerIdStr);

        
        var existing = await _companyService.GetByIdAsync(id);
        if (!existing.Success || existing.Data is null)
            return StatusCode((int)existing.StatusCode, existing);

        if (existing.Data.OwnerId != ownerId)
            return Forbid(); 

        
        var response = await _companyService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

}
