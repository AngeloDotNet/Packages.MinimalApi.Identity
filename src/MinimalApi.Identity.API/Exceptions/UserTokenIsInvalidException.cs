namespace MinimalApi.Identity.API.Exceptions;

public class UserTokenIsInvalidException : Exception
{
    public UserTokenIsInvalidException()
    { }

    public UserTokenIsInvalidException(string? message) : base(message)
    { }

    public UserTokenIsInvalidException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
