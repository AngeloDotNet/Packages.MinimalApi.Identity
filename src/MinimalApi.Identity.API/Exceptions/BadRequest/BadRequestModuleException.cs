﻿namespace MinimalApi.Identity.API.Exceptions.BadRequest;

public class BadRequestModuleException : Exception
{
    public BadRequestModuleException()
    { }

    public BadRequestModuleException(string? message) : base(message)
    { }

    public BadRequestModuleException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
