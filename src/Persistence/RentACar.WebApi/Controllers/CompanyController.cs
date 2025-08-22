using System.Net;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CompanyDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Yeni şirkət yaradır
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CompanyCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _companyService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Şirkəti Id ilə gətirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _companyService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Bütün şirkətləri gətirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _companyService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Şirkəti yeniləyir
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CompanyUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _companyService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Şirkəti silir
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _companyService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Filtrlənmiş şirkət siyahısı gətirir
    /// </summary>
    [HttpPost("filter post")]
    public async Task<IActionResult> GetByFilter([FromBody] CompanyFilterDto filter)
    {
        var response = await _companyService.GetByFilterAsync(filter);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Filtrlənmiş şirkət siyahısı gətirir (query string ilə)
    /// Məs: /api/company/filter?name=Tesla&address=Baku
    /// </summary>
    [HttpGet("filter get")]
    public async Task<IActionResult> GetByFilter(
        [FromQuery] string? name,
        [FromQuery] string? registrationNumber,
        [FromQuery] string? address,
        [FromQuery] Guid? ownerId)
    {
        var filter = new CompanyFilterDto
        {
            Name = name,
            RegistrationNumber = registrationNumber,
            Address = address,
            OwnerId = ownerId
        };

        var response = await _companyService.GetByFilterAsync(filter);
        return StatusCode((int)response.StatusCode, response);
    }
}
