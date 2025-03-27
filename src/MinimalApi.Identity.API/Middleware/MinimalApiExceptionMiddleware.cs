using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Exceptions;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Middleware;

public class MinimalApiExceptionMiddleware(RequestDelegate next, IOptions<ValidationOptions> options)
{
    private readonly ValidationOptions validationOptions = options.Value;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, validationOptions);
        }
    }

    //private static Task HandleExceptionAsync(HttpContext context, Exception exception, ValidationOptions validationOptions)
    //{
    //    ProblemDetails problemDetails;

    //    switch (exception)
    //    {
    //        case BadRequestProfileException badRequestProfileException:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.BadRequest, badRequestProfileException.Message);
    //            problemDetails.Status = (int)HttpStatusCode.BadRequest;
    //            break;
    //        case NotFoundProfileException notFoundProfileException:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.NotFound, notFoundProfileException.Message);
    //            problemDetails.Status = (int)HttpStatusCode.NotFound;
    //            break;
    //        case ArgumentOutOfRangeException argumentOutOfRangeException:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.BadRequest, argumentOutOfRangeException.Message);
    //            problemDetails.Status = (int)HttpStatusCode.BadRequest;
    //            break;
    //        case ArgumentNullException argumentNullException:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.BadRequest, argumentNullException.Message);
    //            problemDetails.Status = (int)HttpStatusCode.BadRequest;
    //            break;
    //        case UserUnknownException userUnknownException:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.Unauthorized, MessageApi.UserNotAuthenticated);
    //            problemDetails.Status = (int)HttpStatusCode.Unauthorized;
    //            break;
    //        case UserWithoutPermissionsException userWithoutPermissionsException:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.Unauthorized, MessageApi.UserNotHavePermission);
    //            problemDetails.Status = (int)HttpStatusCode.Unauthorized;
    //            break;
    //        case ValidationModelException validationException:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.UnprocessableEntity, validationException.Message);
    //            problemDetails.Status = (int)HttpStatusCode.UnprocessableEntity;

    //            switch (validationOptions.ErrorResponseFormat)
    //            {
    //                case ErrorResponseFormat.Default:
    //                    problemDetails.Extensions["errors"] = validationException.Errors;
    //                    break;
    //                case ErrorResponseFormat.List:
    //                    problemDetails.Extensions["errors"] = validationException.Errors
    //                        .SelectMany(e => e.Value.Select(m => new { Name = e.Key, Message = m }))
    //                        .ToArray();
    //                    break;
    //            }

    //            break;
    //        default:
    //            problemDetails = CreateProblemDetails(context, HttpStatusCode.InternalServerError, MessageApi.UnexpectedError);
    //            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
    //            break;
    //    }

    //    context.Response.ContentType = "application/json";
    //    context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

    //    var json = JsonSerializer.Serialize(problemDetails);
    //    return context.Response.WriteAsync(json);
    //}

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ValidationOptions validationOptions)
    {
        var statusCode = exception switch
        {
            BadRequestProfileException => HttpStatusCode.BadRequest,
            NotFoundProfileException => HttpStatusCode.NotFound,
            ArgumentOutOfRangeException => HttpStatusCode.BadRequest,
            ArgumentNullException => HttpStatusCode.BadRequest,
            UserUnknownException => HttpStatusCode.Unauthorized,
            UserWithoutPermissionsException => HttpStatusCode.Unauthorized,
            ValidationModelException => HttpStatusCode.UnprocessableEntity,
            _ => HttpStatusCode.InternalServerError
        };

        var message = exception switch
        {
            BadRequestProfileException badRequestProfileException => badRequestProfileException.Message,
            NotFoundProfileException notFoundProfileException => notFoundProfileException.Message,
            ArgumentOutOfRangeException argumentOutOfRangeException => argumentOutOfRangeException.Message,
            ArgumentNullException argumentNullException => argumentNullException.Message,
            UserUnknownException => MessageApi.UserNotAuthenticated,
            UserWithoutPermissionsException => MessageApi.UserNotHavePermission,
            ValidationModelException validationModelException => validationModelException.Message,
            _ => MessageApi.UnexpectedError
        };

        var problemDetails = CreateProblemDetails(context, statusCode, message);
        problemDetails.Status = (int)statusCode;

        if (exception is ValidationModelException validationException)
        {
            problemDetails.Extensions["errors"] = validationOptions.ErrorResponseFormat switch
            {
                ErrorResponseFormat.List => validationException.Errors
                    .SelectMany(e => e.Value.Select(m => new { Name = e.Key, Message = m }))
                    .ToArray(),
                _ => validationException.Errors
            };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }

    public static ProblemDetails CreateProblemDetails(HttpContext context, HttpStatusCode statusCode, string detail)
    {
        var user = context.Features.Get<IHttpAuthenticationFeature>()?.User;
        var type = $"https://httpstatuses.io/{(int)statusCode}";

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Type = type,
            Title = MessageApi.ProblemDetailsMessageTitle,
            Instance = $"{context.Request.Method} {context.Request.Path}",
            Detail = detail,
            Extensions = {
                ["traceId"] = context.Features.Get<IHttpActivityFeature>()?.Activity.Id,
                ["requestId"] = context.TraceIdentifier,
                ["dateTime"] = DateTime.UtcNow
            }
        };

        if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            var stackTrace = context.Features.Get<IExceptionHandlerFeature>()?.Error?.StackTrace;

            if (!string.IsNullOrEmpty(stackTrace))
            {
                problemDetails.Extensions["stackTrace"] = stackTrace;
            }
        }

        if (user?.Identity?.IsAuthenticated == true)
        {
            var claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);

            if (claims.TryGetValue(ClaimTypes.NameIdentifier, out var userId))
            {
                problemDetails.Extensions["userId"] = userId;
            }

            if (claims.TryGetValue(ClaimTypes.Name, out var userName))
            {
                problemDetails.Extensions["userName"] = userName;
            }
        }

        return problemDetails;
    }
}