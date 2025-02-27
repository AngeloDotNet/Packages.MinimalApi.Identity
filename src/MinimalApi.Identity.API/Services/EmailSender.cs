using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class EmailSender(IOptionsMonitor<SmtpOptions> smtpOptionsMonitor) : IEmailSender
{
    public async Task SendEmailAsync(string emailTo, string emailSubject, string emailMessage)
    {
        var options = smtpOptionsMonitor.CurrentValue;

        using SmtpClient client = new();

        await client.ConnectAsync(options.Host, options.Port, options.Security);

        if (!string.IsNullOrEmpty(options.Username))
        {
            await client.AuthenticateAsync(options.Username, options.Password);
        }

        MimeMessage message = new();

        message.From.Add(MailboxAddress.Parse(options.Sender));
        message.To.Add(MailboxAddress.Parse(emailTo));
        message.Subject = emailSubject;
        message.Body = new TextPart("html")
        {
            Text = emailMessage
        };

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}