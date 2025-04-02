using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.API.Exceptions;

public class BadRequestRoleException : Exception
{
    public BadRequestRoleException()
    { }

    public BadRequestRoleException(string? message) : base(message)
    { }

    public BadRequestRoleException(IEnumerable<IdentityError> errors) : base(FormatErrorMessage(errors))
    { }

    public BadRequestRoleException(IEnumerable<IdentityError> errors, Exception? innerException) : base(FormatErrorMessage(errors), innerException)
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
