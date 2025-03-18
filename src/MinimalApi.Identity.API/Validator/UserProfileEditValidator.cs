using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator;

public class UserProfileEditValidator : AbstractValidator<UserProfileEditModel>
{
    public UserProfileEditValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            //.MaximumLength(50).WithMessage("First name must not exceed 50 characters")
            ;

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            //.MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
            ;
    }
}
