using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.BookingDtos;

public class BookingGetDto
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public string CarBrand { get; set; } = null!;
    public string CarModel { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

