namespace MinimalApi.Identity.API.Exceptions.NotFound;

public class NotFoundModuleException : Exception
{
    public NotFoundModuleException()
    { }

    public NotFoundModuleException(string? message) : base(message)
    { }

    public NotFoundModuleException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
