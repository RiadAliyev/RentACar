namespace RentACar.Application.DTOs.PaymentDtos;

public class PaymentGetDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = null!; // enum string kimi
    public bool IsSuccessful { get; set; }
    public DateTime PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Əlavə məlumat
    public decimal? BookingTotalPrice { get; set; }
}

