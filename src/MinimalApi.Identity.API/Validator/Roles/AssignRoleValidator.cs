using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Roles;

public class AssignRoleValidator : AbstractValidator<AssignRoleModel>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required");
    }
}
