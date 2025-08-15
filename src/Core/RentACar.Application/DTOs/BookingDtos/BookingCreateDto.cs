using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.BookingDtos;

public class BookingCreateDto
{
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public BookingStatus Status { get; set; }
}

