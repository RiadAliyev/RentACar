namespace RentACar.Application.DTOs.PaymentDtos;

public class PaymentGetDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = null!; 
    public bool IsSuccessful { get; set; }
    public DateTime PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal? BookingTotalPrice { get; set; }
}

