namespace MinimalApi.Identity.API.Exceptions.NotFound;

public class NotFoundClaimException : Exception
{
    public NotFoundClaimException()
    { }

    public NotFoundClaimException(string? message) : base(message)
    { }

    public NotFoundClaimException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
