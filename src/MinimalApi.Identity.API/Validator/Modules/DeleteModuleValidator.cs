using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Modules;

public class DeleteModuleValidator : AbstractValidator<DeleteModuleModel>
{
    public DeleteModuleValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("ModuleId is required")
            .GreaterThan(0).WithMessage("ModuleId must be an integer greater than zero");
    }
}
