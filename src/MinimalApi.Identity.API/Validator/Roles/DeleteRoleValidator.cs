﻿using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator.Roles;

public class DeleteRoleValidator : AbstractValidator<DeleteRoleModel>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required");
    }
}
