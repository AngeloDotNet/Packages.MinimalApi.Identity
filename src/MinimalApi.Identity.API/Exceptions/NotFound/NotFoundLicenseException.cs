﻿namespace MinimalApi.Identity.API.Exceptions.NotFound;

public class NotFoundLicenseException : Exception
{
    public NotFoundLicenseException()
    { }

    public NotFoundLicenseException(string? message) : base(message)
    { }

    public NotFoundLicenseException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
