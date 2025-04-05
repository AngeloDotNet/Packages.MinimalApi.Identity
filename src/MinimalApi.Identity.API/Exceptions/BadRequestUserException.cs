namespace MinimalApi.Identity.API.Exceptions;

public class BadRequestUserException : Exception
{
    public BadRequestUserException()
    { }

    public BadRequestUserException(string? message) : base(message)
    { }

    public BadRequestUserException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
