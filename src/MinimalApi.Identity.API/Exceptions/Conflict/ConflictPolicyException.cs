namespace MinimalApi.Identity.API.Exceptions.Conflict;

public class ConflictPolicyException : Exception
{
    public ConflictPolicyException()
    { }

    public ConflictPolicyException(string? message) : base(message)
    { }

    public ConflictPolicyException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
