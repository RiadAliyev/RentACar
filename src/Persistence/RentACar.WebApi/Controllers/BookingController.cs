using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.BookingDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    
    [HttpGet]
    [Authorize(Policy = Permissions.Booking.GetAll)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _bookingService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    
    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permissions.Booking.GetById)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _bookingService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

   
    [HttpPost]
    [Authorize(Policy = Permissions.Booking.Create)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _bookingService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.Booking.Update)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] BookingUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _bookingService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    
    [HttpDelete("Admin")]
    [Authorize(Policy = Permissions.Booking.Delete)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _bookingService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Booking.Cancel)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var r = await _bookingService.CancelAsync(id, byAdmin: false);
        return StatusCode((int)r.StatusCode, r);
    }
}
