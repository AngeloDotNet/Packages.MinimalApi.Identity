using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Extensions;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class EmailSenderService(IConfiguration configuration, IEmailSavingService emailSaving) : IEmailSenderService
{
    public async Task SendEmailTypeAsync(string email, string callbackUrl, int typeSender)
    {
        await SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.", typeSender);
    }

    private async Task SendEmailAsync(string emailTo, string emailSubject, string emailMessage, int emailSendingType)
    {
        try
        {
            var options = configuration.GetSettingsOptions<SmtpOptions>(nameof(SmtpOptions));

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

            if (options.SaveEmailSent)
            {
                var emailSending = new EmailSending
                {
                    EmailSendingTypeId = emailSendingType,
                    EmailTo = emailTo,
                    Subject = emailSubject,
                    Body = emailMessage,
                    Sent = true,
                    DateSent = DateTime.UtcNow
                };

                await emailSaving.SaveEmailAsync(emailSending);
            }
        }
        catch (Exception ex)
        {
            var emailError = new EmailSending
            {
                EmailSendingTypeId = emailSendingType,
                EmailTo = emailTo,
                Subject = emailSubject,
                Body = emailMessage,
                Sent = false,
                DateSent = DateTime.UtcNow,
                ErrorMessage = "Error sending email",
                ErrorDetails = ex.Message
            };

            await emailSaving.SaveEmailAsync(emailError);
        }
    }
}