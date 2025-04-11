namespace MinimalApi.Identity.API.Exceptions.Users;

public class UserUnknownException : Exception
{
    public UserUnknownException()
    { }

    public UserUnknownException(string? message) : base(message)
    { }

    public UserUnknownException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
