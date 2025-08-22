using System.Net;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarImageDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarImageController : ControllerBase
{
    private readonly ICarImageService _carImageService;

    public CarImageController(ICarImageService carImageService)
    {
        _carImageService = carImageService;
    }

    /// <summary>
    /// Yeni car image əlavə edir
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarImageCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carImageService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Id ilə car image gətirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _carImageService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Bütün car image-ləri gətirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _carImageService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Mövcud car image-i update edir
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CarImageUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carImageService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Car image silir
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _carImageService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }
}
