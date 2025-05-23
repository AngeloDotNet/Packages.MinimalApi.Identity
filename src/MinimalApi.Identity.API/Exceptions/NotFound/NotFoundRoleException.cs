﻿namespace MinimalApi.Identity.API.Exceptions.NotFound;

public class NotFoundRoleException : Exception
{
    public NotFoundRoleException()
    { }

    public NotFoundRoleException(string? message) : base(message)
    { }

    public NotFoundRoleException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
