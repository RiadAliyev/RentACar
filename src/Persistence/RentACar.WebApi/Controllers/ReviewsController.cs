using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.ReviewDtos;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
    {
        var result = await _reviewService.CreateAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ReviewUpdateDto dto)
    {
        var result = await _reviewService.UpdateAsync(id, dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _reviewService.DeleteAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _reviewService.GetByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("car/{carId}")]
    public async Task<IActionResult> GetAllByCarId(Guid carId)
    {
        var result = await _reviewService.GetAllByCarIdAsync(carId);
        return StatusCode((int)result.StatusCode, result);
    }
}
