using RentACar.Domain.Enums;

namespace RentACar.Domain.Entities;

public class Booking: BaseEntity
{
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public BookingStatus Status { get; set; }

    public Car Car { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

