using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Validations.Filters;

namespace MinimalApi.Identity.API.Validations.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
        => builder.AddEndpointFilter<ValidatorFilter<T>>().ProducesValidationProblem();
}