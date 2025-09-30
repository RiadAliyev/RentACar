using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.BookingDtos;

public class BookingCreateDto
{
    public Guid CarId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

