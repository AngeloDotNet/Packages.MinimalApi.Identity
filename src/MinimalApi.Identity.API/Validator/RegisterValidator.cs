using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            //.MaximumLength(50).WithMessage("First name must not exceed 50 characters")
            ;

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            //.MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
            ;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            //.MaximumLength(50).WithMessage("Username must not exceed 50 characters")
            ;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            //.MinimumLength(6).WithMessage("Password must be at least 6 characters")
            ;
    }
}
