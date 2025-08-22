using FluentValidation;
using RentACar.Application.DTOs.ReviewDtos;

namespace RentACar.Application.Validations;

public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateDtoValidator()
    {
        RuleFor(x => x.CarId)
            .NotEmpty().WithMessage("CarId is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment cannot be empty.")
            .MinimumLength(5).WithMessage("Comment must be at least 5 characters long.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
    }
}

public class ReviewUpdateDtoValidator : AbstractValidator<ReviewUpdateDto>
{
    public ReviewUpdateDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment cannot be empty.")
            .MinimumLength(5).WithMessage("Comment must be at least 5 characters long.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
    }
}

