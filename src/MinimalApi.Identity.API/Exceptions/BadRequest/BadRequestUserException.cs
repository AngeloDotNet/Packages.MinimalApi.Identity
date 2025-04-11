using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.API.Exceptions.BadRequest;

public class BadRequestUserException : Exception
{
    public BadRequestUserException()
    { }

    public BadRequestUserException(string? message) : base(message)
    { }

    public BadRequestUserException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public BadRequestUserException(IEnumerable<IdentityError> errors) : base(FormatErrorMessage(errors))
    { }

    private static string FormatErrorMessage(IEnumerable<IdentityError> errors)
    {
        var sb = new StringBuilder();

        foreach (var error in errors)
        {
            sb.AppendLine($"{error.Code}: {error.Description}");
        }

        return sb.ToString();
    }
}
