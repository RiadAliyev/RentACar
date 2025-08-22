using System.Net;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarFeatureDto;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarFeatureController : ControllerBase
{
    private readonly ICarFeatureService _carFeatureService;

    public CarFeatureController(ICarFeatureService carFeatureService)
    {
        _carFeatureService = carFeatureService;
    }

    /// <summary>
    /// ✅ Yeni CarFeature əlavə edir
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarFeatureCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carFeatureService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// ✅ Id ilə CarFeature gətirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _carFeatureService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// ✅ Bütün CarFeature-ları gətirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _carFeatureService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// ✅ CarFeature update edir
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CarFeatureUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carFeatureService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// ✅ CarFeature silir
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _carFeatureService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }
}
