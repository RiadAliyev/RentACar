using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/owner/bookings")]
[ApiController]
[Authorize(Policy = Permissions.Booking.ReadForOwner)]
public class OwnerBookingsController : ControllerBase
{
    private readonly IBookingService _service;
    public OwnerBookingsController(IBookingService service) => _service = service;

    
    [HttpGet("pending")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetPending()
    {
        var r = await _service.GetPendingForOwnerAsync();
        return StatusCode((int)r.StatusCode, r);
    }

   
    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = Permissions.Booking.Approve)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var r = await _service.ApproveAsync(id);
        return StatusCode((int)r.StatusCode, r);
    }

    
    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = Permissions.Booking.Reject)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Reject(Guid id)
    {
        var r = await _service.RejectAsync(id);
        return StatusCode((int)r.StatusCode, r);
    }

    
    [HttpPost("{id:guid}/complete")]
    [Authorize(Policy = Permissions.Booking.Complete)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var r = await _service.CompleteAsync(id);
        return StatusCode((int)r.StatusCode, r);
    }
}
