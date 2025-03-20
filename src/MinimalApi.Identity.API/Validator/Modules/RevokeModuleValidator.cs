﻿using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Modules;

public class RevokeModuleValidator : AbstractValidator<RevokeModuleModel>
{
    public RevokeModuleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");

        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("ModuleId is required")
            .GreaterThan(0).WithMessage("ModuleId must be an integer greater than zero");
    }
}
