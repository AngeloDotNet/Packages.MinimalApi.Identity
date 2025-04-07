﻿namespace MinimalApi.Identity.API.Exceptions;

public class BadRequestClaimException : Exception
{
    public BadRequestClaimException()
    { }

    public BadRequestClaimException(string? message) : base(message)
    { }

    public BadRequestClaimException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
