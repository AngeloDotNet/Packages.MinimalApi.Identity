using System.ComponentModel.DataAnnotations;
using MailKit.Security;
using MinimalApi.Identity.API.Validator.Attribute;

namespace MinimalApi.Identity.API.Options;

public class SmtpOptions
{
    [Required(ErrorMessage = "Host is required.")]
    public string Host { get; init; } = null!;

    //[Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
    //public int Port { get; init; }
    [IntegerInList(25, 587, 465, ErrorMessage = "Port must be one of the following: 25, 587, 465.")]
    public int Port { get; init; }

    [Required(ErrorMessage = "Security option is required.")]
    public SecureSocketOptions Security { get; init; }

    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; init; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = null!;

    [Required(ErrorMessage = "Sender is required.")]
    [EmailAddress(ErrorMessage = "Sender must be a valid email address.")]
    public string Sender { get; init; } = null!;

    public bool SaveEmailSent { get; init; }
}
