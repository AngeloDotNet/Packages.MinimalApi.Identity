using MailKit.Security;

namespace MinimalApi.Identity.API.Options;

public class SmtpOptions
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public SecureSocketOptions Security { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Sender { get; init; } = null!;
    public bool SaveEmailSent { get; init; }
}