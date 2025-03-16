using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator;

public class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            //.MaximumLength(50).WithMessage("Username must not exceed 50 characters")
            ;

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            //.MinimumLength(6).WithMessage("Password must be at least 6 characters")
            ;
    }
}
