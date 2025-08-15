namespace RentACar.Application.DTOs.CarDtos;

public class CarGetDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = null!;
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public short Year { get; set; }
    public string TransmissionType { get; set; } = null!;
    public string FuelType { get; set; } = null!;
    public byte Seats { get; set; }
    public decimal DailyPrice { get; set; }
    public string Location { get; set; } = null!;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }

    // Əlavə məlumatlar
    public List<string> Features { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}

