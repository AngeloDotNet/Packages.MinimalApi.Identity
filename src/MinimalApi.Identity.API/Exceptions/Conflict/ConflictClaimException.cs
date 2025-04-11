namespace MinimalApi.Identity.API.Exceptions.Conflict;

public class ConflictClaimException : Exception
{
    public ConflictClaimException()
    { }

    public ConflictClaimException(string? message) : base(message)
    { }

    public ConflictClaimException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
