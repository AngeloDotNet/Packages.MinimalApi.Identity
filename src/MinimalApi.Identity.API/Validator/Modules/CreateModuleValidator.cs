using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Modules;

public class CreateModuleValidator : AbstractValidator<CreateModuleModel>
{
    public CreateModuleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            //.MaximumLength(50).WithMessage("Name must not exceed 50 characters")
            ;

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            //.MaximumLength(100).WithMessage("Description must not exceed 100 characters")
            ;
    }
}
