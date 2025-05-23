﻿using FluentValidation;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Validator;

public class ImpersonateUserValidator : AbstractValidator<ImpersonateUserModel>
{
    public ImpersonateUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");
    }
}