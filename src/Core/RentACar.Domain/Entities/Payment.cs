using RentACar.Domain.Enums;

namespace RentACar.Domain.Entities;

public class Payment: BaseEntity
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public Booking Booking { get; set; } = null!;
}

