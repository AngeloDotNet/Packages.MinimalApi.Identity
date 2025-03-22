using FluentValidation;
using Microsoft.Extensions.Configuration;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator.Licenses;

public class CreateLicenseValidator : AbstractValidator<CreateLicenseModel>
{
    public CreateLicenseValidator(IConfiguration configuration)
    {
        var validationOptions = configuration.GetSettingsOptions<ApiValidationOptions>(nameof(ApiValidationOptions));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(validationOptions.MinLengthLicenseName).WithMessage($"Name must be at least {validationOptions.MinLengthLicenseName} characters")
            .MaximumLength(validationOptions.MaxLengthLicenseName).WithMessage($"Name must not exceed {validationOptions.MaxLengthLicenseName} characters");

        RuleFor(x => x.ExpirationDate)
            .NotEmpty().WithMessage("Expiration date is required")
            .Must(x => x > DateOnly.FromDateTime(DateTime.Now)).WithMessage("Expiration date must be greater than today");
    }
}