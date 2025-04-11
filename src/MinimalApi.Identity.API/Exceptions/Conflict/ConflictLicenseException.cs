namespace MinimalApi.Identity.API.Exceptions.Conflict;

public class ConflictLicenseException : Exception
{
    public ConflictLicenseException()
    { }

    public ConflictLicenseException(string? message) : base(message)
    { }

    public ConflictLicenseException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
