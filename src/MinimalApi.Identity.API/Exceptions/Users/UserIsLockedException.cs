namespace MinimalApi.Identity.API.Exceptions.Users;

public class UserIsLockedException : Exception
{
    public UserIsLockedException()
    { }

    public UserIsLockedException(string? message) : base(message)
    { }

    public UserIsLockedException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
