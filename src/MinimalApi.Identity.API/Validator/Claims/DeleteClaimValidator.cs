using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Claims;

public class DeleteClaimValidator : AbstractValidator<DeleteClaimModel>
{
    public DeleteClaimValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");
    }
}
