namespace MinimalApi.Identity.API.Exceptions.BadRequest;

public class BadRequestPolicyException : Exception
{
    public BadRequestPolicyException()
    { }

    public BadRequestPolicyException(string? message) : base(message)
    { }

    public BadRequestPolicyException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
