using FluentValidation;
using Microsoft.Extensions.Configuration;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator.Profiles;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfileModel>
{
    public CreateUserProfileValidator(IConfiguration configuration)
    {
        var applicationOptions = configuration.GetSettingsOptions<ApplicationOptions>(nameof(ApplicationOptions));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(applicationOptions.MinLengthFirstName).WithMessage($"First name must be at least {applicationOptions.MinLengthFirstName} characters")
            .MaximumLength(applicationOptions.MaxLengthFirstName).WithMessage($"First name must not exceed {applicationOptions.MaxLengthFirstName} characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(applicationOptions.MinLengthLastName).WithMessage($"Last name must be at least {applicationOptions.MinLengthLastName} characters")
            .MaximumLength(applicationOptions.MaxLengthLastName).WithMessage($"Last name must not exceed {applicationOptions.MaxLengthLastName} characters");
    }
}
