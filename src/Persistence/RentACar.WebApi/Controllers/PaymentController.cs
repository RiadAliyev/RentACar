using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.PaymentDtos;

namespace RentACar.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentCreateDto dto)
    {
        var response = await _paymentService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PaymentUpdateDto dto)
    {
        var response = await _paymentService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _paymentService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _paymentService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _paymentService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("by-booking/{bookingId:guid}")]
    public async Task<IActionResult> GetByBookingId(Guid bookingId)
    {
        var response = await _paymentService.GetByBookingIdAsync(bookingId);
        return StatusCode((int)response.StatusCode, response);
    }
}
