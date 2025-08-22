using System.Net;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    /// <summary>
    /// Yeni maşın yaradır
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Maşını Id ilə gətirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _carService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Bütün maşınları gətirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _carService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Mövcud maşını yeniləyir
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CarUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Maşını silir
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _carService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Filtrlənmiş maşın siyahısı gətirir
    /// </summary>
    [HttpPost("filter")]
    public async Task<IActionResult> GetByFilter([FromBody] CarFilterDto filter)
    {
        var response = await _carService.GetByFilterAsync(filter);
        return StatusCode((int)response.StatusCode, response);
    }
}
