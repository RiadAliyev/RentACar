using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.CarDtos;

public class CarGetDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = null!;
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public short Year { get; set; }
    public TransmissionType TransmissionType { get; set; }
    public FuelType FuelType { get; set; }
    public byte Seats { get; set; }
    public decimal DailyPrice { get; set; }
    public string Location { get; set; } = null!;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }

    // Əlavə məlumatlar
    public List<string> Features { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}

