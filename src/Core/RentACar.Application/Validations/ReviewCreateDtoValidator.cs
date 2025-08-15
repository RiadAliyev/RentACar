using FluentValidation;
using RentACar.Application.DTOs.ReviewDtos;

namespace RentACar.Application.Validations;

public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateDtoValidator()
    {
        RuleFor(x => x.CarId).GreaterThan(0);
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(1000);
    }
}

public class ReviewUpdateDtoValidator : AbstractValidator<ReviewUpdateDto>
{
    public ReviewUpdateDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(1000);
    }
}

