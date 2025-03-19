namespace MinimalApi.Identity.API.Exceptions;

public class UserWithoutPermissionsException : Exception
{
    public UserWithoutPermissionsException()
    { }

    public UserWithoutPermissionsException(string? message) : base(message)
    { }

    public UserWithoutPermissionsException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
