namespace MinimalApi.Identity.API.Exceptions.BadRequest;

public class BadRequestLicenseException : Exception
{
    public BadRequestLicenseException()
    { }

    public BadRequestLicenseException(string? message) : base(message)
    { }

    public BadRequestLicenseException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
