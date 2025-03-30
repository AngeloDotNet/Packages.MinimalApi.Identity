using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator.Profiles;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfileModel>
{
    //public CreateUserProfileValidator(IConfiguration configuration)
    public CreateUserProfileValidator(IOptions<ApiValidationOptions> options)
    {
        //var validationOptions = configuration.GetSettingsOptions<ApiValidationOptions>(nameof(ApiValidationOptions));
        var validationOptions = options.Value;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(validationOptions.MinLengthFirstName).WithMessage($"First name must be at least {validationOptions.MinLengthFirstName} characters")
            .MaximumLength(validationOptions.MaxLengthFirstName).WithMessage($"First name must not exceed {validationOptions.MaxLengthFirstName} characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(validationOptions.MinLengthLastName).WithMessage($"Last name must be at least {validationOptions.MinLengthLastName} characters")
            .MaximumLength(validationOptions.MaxLengthLastName).WithMessage($"Last name must not exceed {validationOptions.MaxLengthLastName} characters");
    }
}
