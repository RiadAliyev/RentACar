using FluentValidation;
using RentACar.Application.DTOs.BookingDtos;

namespace RentACar.Application.Validations;

public class BookingCreateDtoValidator : AbstractValidator<BookingCreateDto>
{
    public BookingCreateDtoValidator()
    {
        
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date.");
        RuleFor(x => x.TotalPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0);
    }
}

public class BookingUpdateDtoValidator : AbstractValidator<BookingUpdateDto>
{
    public BookingUpdateDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date.");
        RuleFor(x => x.TotalPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0);
    }
}

