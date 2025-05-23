﻿namespace MinimalApi.Identity.API.Exceptions.NotFound;

public class NotFoundActivePoliciesException : Exception
{
    public NotFoundActivePoliciesException()
    { }

    public NotFoundActivePoliciesException(string? message) : base(message)
    { }

    public NotFoundActivePoliciesException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
