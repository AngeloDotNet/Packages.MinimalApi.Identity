using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator.Modules;

public class CreateModuleValidator : AbstractValidator<CreateModuleModel>
{
    public CreateModuleValidator(IOptions<ApiValidationOptions> options)
    {
        var validationOptions = options.Value;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(validationOptions.MinLengthModuleName).WithMessage($"Name must be at least {validationOptions.MinLengthModuleName} characters")
            .MaximumLength(validationOptions.MaxLengthModuleName).WithMessage($"Name must not exceed {validationOptions.MaxLengthModuleName} characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(validationOptions.MinLengthModuleDescription).WithMessage($"Name must be at least {validationOptions.MinLengthModuleDescription} characters")
            .MaximumLength(validationOptions.MaxLengthModuleDescription).WithMessage($"Name must not exceed {validationOptions.MaxLengthModuleDescription} characters");
    }
}
