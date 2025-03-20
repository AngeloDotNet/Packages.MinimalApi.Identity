using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Exceptions;

namespace MinimalApi.Identity.API.Middleware;

public class MinimalApiExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            case UserUnknownException userUnknownException:
                problemDetails = CreateProblemDetails(context, HttpStatusCode.Unauthorized, MessageApi.UserNotAuthenticated);
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                break;
            case UserWithoutPermissionsException userWithoutPermissionsException:
                problemDetails = CreateProblemDetails(context, HttpStatusCode.Unauthorized, MessageApi.UserNotHavePermission);
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                break;
            case ValidationModelException validationException:
                problemDetails = CreateProblemDetails(context, HttpStatusCode.UnprocessableEntity, validationException.Message);
                problemDetails.Status = (int)HttpStatusCode.UnprocessableEntity;
                problemDetails.Extensions["errors"] = validationException.Errors
                    .SelectMany(e => e.Value.Select(m => new { Name = e.Key, Message = m })).ToArray();
                break;
            default:
                problemDetails = CreateProblemDetails(context, HttpStatusCode.InternalServerError, MessageApi.UnexpectedError);
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }

    public static ProblemDetails CreateProblemDetails(HttpContext context, HttpStatusCode statusCode, string detail)
    {
        var user = context.Features.Get<IHttpAuthenticationFeature>()?.User;
        var type = $"https://httpstatuses.io/{statusCode}";

        var title = MessageApi.ProblemDetailsMessageTitle;
        var instance = $"{context.Request.Method} {context.Request.Path}";

        var traceId = context.Features.Get<IHttpActivityFeature>()?.Activity.Id;
        var requestId = context.TraceIdentifier;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Type = type,
            Title = title,
            Instance = instance,
            Detail = detail,
            Extensions = {
                ["traceId"] = traceId,
                ["requestId"] = requestId,
                ["dateTime"] = DateTime.UtcNow
            }
        };

        if (user?.Identity?.IsAuthenticated == true)
        {
            problemDetails.Extensions["userId"] = user.FindFirstValue(ClaimTypes.NameIdentifier);
            problemDetails.Extensions["userName"] = user.FindFirstValue(ClaimTypes.Name);
        }

        return problemDetails;
    }
}