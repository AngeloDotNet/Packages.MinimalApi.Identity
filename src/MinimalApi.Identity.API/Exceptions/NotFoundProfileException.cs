namespace MinimalApi.Identity.API.Exceptions;

public class NotFoundProfileException : Exception
{
    public NotFoundProfileException()
    { }

    public NotFoundProfileException(string? message) : base(message)
    { }

    public NotFoundProfileException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
