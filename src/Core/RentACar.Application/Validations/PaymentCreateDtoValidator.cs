using FluentValidation;
using RentACar.Application.DTOs.PaymentDtos;

namespace RentACar.Application.Validations;

public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateDtoValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking seçilməlidir.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Ödəniş məbləği 0-dan böyük olmalıdır.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Ödəniş metodu düzgün seçilməyib.");

        RuleFor(x => x.PaidAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Gələcək tarixdə ödəniş edilə bilməz.");
    }
}

public class PaymentUpdateDtoValidator : AbstractValidator<PaymentUpdateDto>
{
    public PaymentUpdateDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Ödəniş məbləği 0-dan böyük olmalıdır.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Ödəniş metodu düzgün seçilməyib.");

        RuleFor(x => x.PaidAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Gələcək tarixdə ödəniş edilə bilməz.");
    }
}

