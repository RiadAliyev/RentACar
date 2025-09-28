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
    public async Task<IActionResult> GetAll()
    {
        var response = await _companyService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }


    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _companyService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CompanyCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("validation error", HttpStatusCode.BadRequest));

        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(ownerIdStr))
            return Unauthorized(BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized));

        var ownerId = Guid.Parse(ownerIdStr);

        var response = await _companyService.CreateAsync(dto, ownerId); // <<-- yeni imza
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CompanyOwnerOnly")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CompanyUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("validation error", HttpStatusCode.BadRequest));

        // 1) Token-dən userId al
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(ownerIdStr))
            return Unauthorized(BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized));
        var ownerId = Guid.Parse(ownerIdStr);

        // 2) Sahibliyi yoxla
        var existing = await _companyService.GetByIdAsync(id);
        if (!existing.Success || existing.Data is null)
            return StatusCode((int)existing.StatusCode, existing);

        if (existing.Data.OwnerId != ownerId)
            return Forbid(); // 403 – başqasının şirkətini dəyişmək olmaz

        // 3) Update
        var response = await _companyService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CompanyOwnerOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // 1) Token-dən userId al
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(ownerIdStr))
            return Unauthorized(BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized));
        var ownerId = Guid.Parse(ownerIdStr);

        // 2) Sahibliyi yoxla
        var existing = await _companyService.GetByIdAsync(id);
        if (!existing.Success || existing.Data is null)
            return StatusCode((int)existing.StatusCode, existing);

        if (existing.Data.OwnerId != ownerId)
            return Forbid(); // 403

        // 3) Delete
        var response = await _companyService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

}
