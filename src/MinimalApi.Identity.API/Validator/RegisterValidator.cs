using FluentValidation;
using Microsoft.Extensions.Configuration;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    private static int requiredUniqueChars;

    public RegisterValidator(IConfiguration configuration)
    {
        var identityOptions = configuration.GetSettingsOptions<NetIdentityOptions>(nameof(NetIdentityOptions));
        var applicationOptions = configuration.GetSettingsOptions<ApplicationOptions>(nameof(ApplicationOptions));

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(applicationOptions.MinLengthFirstName).WithMessage($"First name must be at least {applicationOptions.MinLengthFirstName} characters")
            .MaximumLength(applicationOptions.MaxLengthFirstName).WithMessage($"First name must not exceed {applicationOptions.MaxLengthFirstName} characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(applicationOptions.MinLengthLastName).WithMessage($"Last name must be at least {applicationOptions.MinLengthLastName} characters")
            .MaximumLength(applicationOptions.MaxLengthLastName).WithMessage($"Last name must not exceed {applicationOptions.MaxLengthLastName} characters");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(applicationOptions.MinLengthUsername).WithMessage($"Username must be at least {applicationOptions.MinLengthUsername} characters")
            .MaximumLength(applicationOptions.MaxLengthUsername).WithMessage($"Username must not exceed {applicationOptions.MaxLengthUsername} characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        requiredUniqueChars = identityOptions.RequiredUniqueChars;

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(identityOptions.RequiredLength).WithMessage($"Password must be at least {identityOptions.RequiredLength} characters")
            .Must(ContainAtLeastTwoUppercaseLetters).WithMessage("Password must contain at least one uppercase letter.")
            .Must(ContainAtLeastOneLowercaseLetter).WithMessage("Password must contain at least one lowercase letter.")
            .Must(ContainAtLeastOneNonAlphanumericCharacter).WithMessage("Password must contain at least one non-alphanumeric character.")
            .Must(ContainAtLeastUniqueCharacters).WithMessage($"Password must contain at least {requiredUniqueChars} unique characters.");
    }

    private bool ContainAtLeastTwoUppercaseLetters(string password)
    {
        return password.Count(char.IsUpper) >= 0;
    }

    private bool ContainAtLeastOneLowercaseLetter(string password)
    {
        return password.Count(char.IsLower) >= 0;
    }

    private bool ContainAtLeastOneNonAlphanumericCharacter(string password)
    {
        return password.Any(ch => !char.IsLetterOrDigit(ch));
    }

    private bool ContainAtLeastUniqueCharacters(string password)
    {
        return password.Distinct().Count() >= requiredUniqueChars;
    }
}
