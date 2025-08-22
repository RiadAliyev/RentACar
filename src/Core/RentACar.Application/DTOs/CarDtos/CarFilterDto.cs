using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.CarDtos;

public class CarFilterDto
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public short? Year { get; set; }
    public TransmissionType? TransmissionType { get; set; }
    public FuelType? FuelType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Location { get; set; }
    public bool? IsApproved { get; set; }
}
