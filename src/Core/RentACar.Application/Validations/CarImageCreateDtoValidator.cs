using FluentValidation;
using RentACar.Application.DTOs.CarImageDtos;

namespace RentACar.Application.Validations;

public class CarImageCreateDtoValidator : AbstractValidator<CarImageCreateDto>
{
    public CarImageCreateDtoValidator()
    {
        
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid image URL format.");
    }
}

public class CarImageUpdateDtoValidator : AbstractValidator<CarImageUpdateDto>
{
    public CarImageUpdateDtoValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid image URL format.");
    }
}

