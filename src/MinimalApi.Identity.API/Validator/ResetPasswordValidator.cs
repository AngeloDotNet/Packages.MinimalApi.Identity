using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordModel>
{
    public ResetPasswordValidator(IOptions<NetIdentityOptions> iOptions)
    {
        var identityOptions = iOptions.Value;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(identityOptions.RequiredLength).WithMessage($"Password must be at least {identityOptions.RequiredLength} characters");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");
    }
}