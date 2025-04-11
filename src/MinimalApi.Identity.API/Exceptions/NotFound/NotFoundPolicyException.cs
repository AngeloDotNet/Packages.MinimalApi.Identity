namespace MinimalApi.Identity.API.Exceptions.NotFound;

public class NotFoundPolicyException : Exception
{
    public NotFoundPolicyException()
    { }

    public NotFoundPolicyException(string? message) : base(message)
    { }

    public NotFoundPolicyException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
