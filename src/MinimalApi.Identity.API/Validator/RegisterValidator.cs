using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    private static int requiredUniqueChars;

    public RegisterValidator(IOptions<NetIdentityOptions> iOptions, IOptions<ApiValidationOptions> vOptions)
    {
        var identityOptions = iOptions.Value;
        var validationOptions = vOptions.Value;

        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(validationOptions.MinLengthFirstName).WithMessage($"First name must be at least {validationOptions.MinLengthFirstName} characters")
            .MaximumLength(validationOptions.MaxLengthFirstName).WithMessage($"First name must not exceed {validationOptions.MaxLengthFirstName} characters");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(validationOptions.MinLengthLastName).WithMessage($"Last name must be at least {validationOptions.MinLengthLastName} characters")
            .MaximumLength(validationOptions.MaxLengthLastName).WithMessage($"Last name must not exceed {validationOptions.MaxLengthLastName} characters");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(validationOptions.MinLengthUsername).WithMessage($"Username must be at least {validationOptions.MinLengthUsername} characters")
            .MaximumLength(validationOptions.MaxLengthUsername).WithMessage($"Username must not exceed {validationOptions.MaxLengthUsername} characters");

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
