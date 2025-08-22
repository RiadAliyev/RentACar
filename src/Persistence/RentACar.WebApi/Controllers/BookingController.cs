using System.Net;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.BookingDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Yeni booking yaradır
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _bookingService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Id ilə booking gətirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _bookingService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Bütün booking-ləri gətirir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _bookingService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Mövcud booking-i yeniləyir
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BookingUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _bookingService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Booking silir
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _bookingService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }
}
