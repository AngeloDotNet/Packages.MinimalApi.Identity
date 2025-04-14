namespace MinimalApi.Identity.API.Exceptions.BadRequest;

public class BadRequestException : Exception
{
    public BadRequestException()
    { }

    public BadRequestException(string? message) : base(message)
    { }

    public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
