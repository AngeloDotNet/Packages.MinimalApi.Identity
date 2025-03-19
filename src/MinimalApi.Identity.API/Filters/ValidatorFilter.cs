using FluentValidation;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Exceptions;

namespace MinimalApi.Identity.API.Filters;

internal class ValidatorFilter<T>(IValidator<T> validator) : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(T)) is not T input)
        {
            return TypedResults.UnprocessableEntity();
        }

        var validationResult = await validator.ValidateAsync(input);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

        var errors = validationResult.ToDictionary();

        throw new ValidationModelException("One or more validation errors occurred", errors);
    }
}