using RentACar.Domain.Enums;

namespace RentACar.Application.DTOs.UserDtos;

public sealed class CarInfoDto
{
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public decimal RentPrice { get; set; } // Car.DailyPrice
}

public sealed class MeProfileDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }

    public AccountType AccountType { get; set; } // Buyer | Owner | Company
    public List<string> Roles { get; set; } = new();
    //public List<string> Permissions { get; set; } = new();

    // Company/Owner üçün əlavə məlumat
    public int? CarCount { get; set; }
    public List<CarInfoDto>? Cars { get; set; }
}
