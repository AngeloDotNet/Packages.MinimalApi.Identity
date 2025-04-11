namespace MinimalApi.Identity.API.Exceptions.Conflict;

public class ConflictRoleException : Exception
{
    public ConflictRoleException()
    { }

    public ConflictRoleException(string? message) : base(message)
    { }

    public ConflictRoleException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
