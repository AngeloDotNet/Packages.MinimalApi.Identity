using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator;

public class ChangeEmailValidator : AbstractValidator<ChangeEmailModel>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New Email is required.")
            .EmailAddress().WithMessage("New Email is not valid.");

        RuleFor(x => x)
            .Custom((model, context) =>
            {
                if (model.Email == model.NewEmail)
                {
                    context.AddFailure("NewEmail", "The two email addresses must be different.");
                }
            });
    }
}
