using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Identity.API.Middleware;

namespace MinimalApi.Identity.Sample.Middleware;

public class ExtendedExceptionMiddleware(RequestDelegate next) : MinimalApiExceptionMiddleware(next)
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