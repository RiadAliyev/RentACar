using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.PaymentDtos;

public class PaymentCreateDto
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}

