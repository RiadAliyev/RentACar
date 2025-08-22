using Microsoft.VisualBasic.FileIO;
using RentACar.Domain.Enums;

namespace RentACar.Domain.Entities;

public class Car: BaseEntity
{
    public Guid OwnerId { get; set; }
    public Guid? CompanyId { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public short Year { get; set; }
    public TransmissionType TransmissionType { get; set; }
    public FuelType FuelType { get; set; }
    public byte Seats { get; set; }
    public decimal DailyPrice { get; set; }
    public string Location { get; set; } = null!;
    public bool IsApproved { get; set; }

    public User Owner { get; set; } = null!;
    public Company? Company { get; set; }
    public ICollection<CarFeatureAssignment> Features { get; set; } = new List<CarFeatureAssignment>();
    public ICollection<CarImage> Images { get; set; } = new List<CarImage>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

