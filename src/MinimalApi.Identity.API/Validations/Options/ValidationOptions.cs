using Microsoft.AspNetCore.Http;

namespace MinimalApi.Identity.API.Validations.Options;

public class ValidationOptions
{
    public ErrorResponseFormat ErrorResponseFormat { get; set; }
    public Func<EndpointFilterInvocationContext, IDictionary<string, string[]>, string>? ValidationErrorTitleMessageFactory { get; set; }
}
