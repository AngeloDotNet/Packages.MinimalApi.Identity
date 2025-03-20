using FluentValidation;
using Microsoft.Extensions.Configuration;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Validator.Modules;

public class CreateModuleValidator : AbstractValidator<CreateModuleModel>
{
    public CreateModuleValidator(IConfiguration configuration)
    {
        var applicationOptions = configuration.GetSettingsOptions<ApplicationOptions>(nameof(ApplicationOptions));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(applicationOptions.MinLengthModuleName).WithMessage($"Name must be at least {applicationOptions.MinLengthModuleName} characters")
            .MaximumLength(applicationOptions.MaxLengthModuleName).WithMessage($"Name must not exceed {applicationOptions.MaxLengthModuleName} characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(applicationOptions.MinLengthModuleDescription).WithMessage($"Name must be at least {applicationOptions.MinLengthModuleDescription} characters")
            .MaximumLength(applicationOptions.MaxLengthModuleDescription).WithMessage($"Name must not exceed {applicationOptions.MaxLengthModuleDescription} characters");
    }
}
