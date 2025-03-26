namespace MinimalApi.Identity.API.Exceptions;

public class BadRequestProfileException : Exception
{
    public BadRequestProfileException()
    { }

    public BadRequestProfileException(string? message) : base(message)
    { }

    public BadRequestProfileException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
