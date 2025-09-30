using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.ReviewDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }


    [HttpGet("car/{carId}")]
    [Authorize(Policy = Permissions.Reviews.GetAllByCarId)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllByCarId(Guid carId)
    {
        var result = await _reviewService.GetAllByCarIdAsync(carId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Reviews.Create)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
    {
        var result = await _reviewService.CreateAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.Reviews.Delete)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _reviewService.DeleteAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }
}
