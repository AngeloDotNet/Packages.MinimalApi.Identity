using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Middleware;
using MinimalApi.Identity.API.Options;
namespace IdentityManager.API.Middleware;

public class ExtendedExceptionMiddleware(RequestDelegate next, IOptions<ValidationOptions> options)
    : MinimalApiExceptionMiddleware(next, options)
{
    public static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            //case CustomException customException:
            //    problemDetails = CreateProblemDetails(context, HttpStatusCode.BadRequest, customException.Message);
            //    problemDetails.Extensions["customProperty"] = customException.CustomProperty;
            //    break;
            default:
                problemDetails = CreateProblemDetails(context, HttpStatusCode.InternalServerError, "An unexpected error occurred!");
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}