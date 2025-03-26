using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Policy;

public class DeletePolicyValidator : AbstractValidator<DeletePolicyModel>
{
    public DeletePolicyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .GreaterThan(0).WithMessage("Id must be an integer greater than zero");

        RuleFor(x => x.PolicyName)
            .NotEmpty().WithMessage("Name is required.");
    }
}