namespace MinimalApi.Identity.API.Exceptions;

public class ConflictModuleException : Exception
{
    public ConflictModuleException()
    { }

    public ConflictModuleException(string? message) : base(message)
    { }

    public ConflictModuleException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
