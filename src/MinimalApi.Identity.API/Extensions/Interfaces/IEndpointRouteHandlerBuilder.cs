using Microsoft.AspNetCore.Routing;

namespace MinimalApi.Identity.Common.Extensions.Interfaces;

public interface IEndpointRouteHandlerBuilder
{
    public static abstract void MapEndpoints(IEndpointRouteBuilder endpoints);
}