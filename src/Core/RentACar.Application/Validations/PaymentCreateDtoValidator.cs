using FluentValidation;
using RentACar.Application.DTOs.PaymentDtos;

namespace RentACar.Application.Validations;

public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateDtoValidator()
    {
        RuleFor(x => x.BookingId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PaymentMethod).IsInEnum();
    }
}

public class PaymentUpdateDtoValidator : AbstractValidator<PaymentUpdateDto>
{
    public PaymentUpdateDtoValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PaymentMethod).IsInEnum();
    }
}

