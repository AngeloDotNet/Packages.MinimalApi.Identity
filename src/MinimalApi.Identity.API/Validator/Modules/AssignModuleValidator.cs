using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Modules;

public class AssignModuleValidator : AbstractValidator<AssignModuleModel>
{
    public AssignModuleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be greater than 0");

        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("ModuleId is required")
            .GreaterThan(0).WithMessage("ModuleId must be greater than 0");
    }
}
