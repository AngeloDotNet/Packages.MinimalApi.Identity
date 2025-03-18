using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Licenses;

public class CreateLicenseValidator : AbstractValidator<CreateLicenseModel>
{
    public CreateLicenseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            //.MaximumLength(50).WithMessage("Name must not exceed 50 characters")
            ;

        RuleFor(x => x.ExpirationDate)
            .NotEmpty().WithMessage("Expiration date is required")
            .Must(x => x > DateOnly.FromDateTime(DateTime.Now)).WithMessage("Expiration date must be greater than today");
    }
}