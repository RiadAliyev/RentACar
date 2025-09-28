using Microsoft.AspNetCore.Http;
using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.CarDtos;

public class CarUpdateDto
{
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

    // Yenilənə bilən əlavə məlumatlar
    public List<Guid>? FeatureIds { get; set; }
    public List<IFormFile>? NewImages { get; set; }
}

