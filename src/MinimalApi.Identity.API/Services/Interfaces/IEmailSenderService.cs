using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IEmailSenderService
{
    /// <summary>
    /// Sends an email with a specific type and a callback URL.
    /// </summary>
   /// <param name="email">The recipient's email address.</param>
   /// <param name="callbackUrl">The callback URL to include in the email.</param>
   /// <param name="typeSender">The type of email to send, as defined by the <see cref="EmailSendingType"/> enum.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   Task SendEmailTypeAsync(string email, string callbackUrl, EmailSendingType typeSender);
   
   /// <summary>
   /// Sends a general-purpose email with a subject and message.
   /// </summary>
   /// <param name="email">The recipient's email address.</param>
   /// <param name="subject">The subject of the email.</param>
   /// <param name="message">The body content of the email.</param>
   /// <param name="typeSender">The type of email to send, as defined by the <see cref="EmailSendingType"/> enum.</param>
   /// <returns>A task that represents the asynchronous operation.</returns>
   Task SendEmailAsync(string email, string subject, string message, EmailSendingType typeSender);
}