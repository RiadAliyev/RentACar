using FluentValidation;
using RentACar.Application.DTOs.CarFeatureDto;

namespace RentACar.Application.Validations;

public class CarFeatureCreateDtoValidator : AbstractValidator<CarFeatureCreateDto>
{
    public CarFeatureCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
    }
}

public class CarFeatureUpdateDtoValidator : AbstractValidator<CarFeatureUpdateDto>
{
    public CarFeatureUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
    }
}

