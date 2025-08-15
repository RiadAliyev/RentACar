using FluentValidation;
using RentACar.Application.DTOs.CarDtos;

namespace RentACar.Application.Validations;

public class CarCreateDtoValidator : AbstractValidator<CarCreateDto>
{
    public CarCreateDtoValidator()
    {
        RuleFor(x => x.OwnerId).GreaterThan(0);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Year)
            .InclusiveBetween((short)1990, (short)(DateTime.UtcNow.Year + 1));
        RuleFor(x => x.Seats).InclusiveBetween((byte)1, (byte)9);
        RuleFor(x => x.DailyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(160);
    }
}

public class CarUpdateDtoValidator : AbstractValidator<CarUpdateDto>
{
    public CarUpdateDtoValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Year)
            .InclusiveBetween((short)1990, (short)(DateTime.UtcNow.Year + 1));
        RuleFor(x => x.Seats).InclusiveBetween((byte)1, (byte)9);
        RuleFor(x => x.DailyPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(160);
    }
}

