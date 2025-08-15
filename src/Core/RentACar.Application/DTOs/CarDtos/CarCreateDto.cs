using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.CarDtos;

public class CarCreateDto
{
    public int OwnerId { get; set; }
    public int? CompanyId { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public short Year { get; set; }
    public TransmissionType TransmissionType { get; set; }
    public FuelType FuelType { get; set; }
    public byte Seats { get; set; }
    public decimal DailyPrice { get; set; }
    public string Location { get; set; } = null!;
    public bool IsApproved { get; set; }

    // Əlavə: xüsusiyyətlər və şəkillər id-lər siyahısı
    public List<int>? FeatureIds { get; set; }
    public List<string>? ImageUrls { get; set; }
}

