namespace MinimalApi.Identity.API.Exceptions;

public class NotFoundModuleException : Exception
{
    public NotFoundModuleException()
    { }

    public NotFoundModuleException(string? message) : base(message)
    { }

    public NotFoundModuleException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
