using FluentValidation;
using RentACar.Application.DTOs.CompanyDtos;

namespace RentACar.Application.Validations;

public class CompanyCreateDtoValidator : AbstractValidator<CompanyCreateDto>
{
    public CompanyCreateDtoValidator()
    {
        
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
    }
}

public class CompanyUpdateDtoValidator : AbstractValidator<CompanyUpdateDto>
{
    public CompanyUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
    }
}
