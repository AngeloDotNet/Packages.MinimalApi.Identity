using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Validations.Options;

namespace MinimalApi.Identity.API.Validations.Filters;

internal class ValidatorFilter<T>(IValidator<T> validator, IOptions<ValidationOptions> options) : IEndpointFilter where T : class
{
    private readonly ValidationOptions validationOptions = options.Value;

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
        var messageError = validationOptions.ValidationErrorTitleMessageFactory?.Invoke(context, errors) ?? "One or more validation errors occurred";

        //var result = TypedResults.Problem(
        //    statusCode: StatusCodes.Status422UnprocessableEntity,
        //    instance: context.HttpContext.Request.Path,
        //    title: validationOptions.ValidationErrorTitleMessageFactory?.Invoke(context, errors) ?? "One or more validation errors occurred",
        //    extensions: new Dictionary<string, object?>(StringComparer.Ordinal)
        //    {
        //        ["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier,
        //        ["errors"] = validationOptions.ErrorResponseFormat == ErrorResponseFormat.Default
        //            ? errors : errors.SelectMany(e
        //                => e.Value.Select(m => new { Name = e.Key, Message = m })).ToArray()
        //    }
        //);

        var statusCode422 = StatusCodes.Status422UnprocessableEntity;
        var result = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{statusCode422}",
            Title = "Validation errors",
            Status = statusCode422,
            Detail = validationOptions.ValidationErrorTitleMessageFactory?.Invoke(context, errors) ?? "One or more validation errors occurred",
            Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}",
            Extensions = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["traceId"] = Activity.Current?.Id,
                ["requestId"] = context.HttpContext.TraceIdentifier,
                //["userId"] = context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                //["userName"] = context.HttpContext.User?.FindFirstValue(ClaimTypes.Name),
                ["dateTime"] = DateTime.UtcNow,
                ["errors"] = validationOptions.ErrorResponseFormat == ErrorResponseFormat.Default ?
                errors : errors.SelectMany(e
                    => e.Value.Select(m => new { Name = e.Key, Message = m })).ToArray()
            }
        };

        return result;
    }
}