namespace MinimalApi.Identity.API.Exceptions;

public class NotFoundUserException : Exception
{
    public NotFoundUserException()
    { }

    public NotFoundUserException(string? message) : base(message)
    { }

    public NotFoundUserException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
