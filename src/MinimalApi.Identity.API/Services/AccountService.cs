using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AccountService(UserManager<ApplicationUser> userManager, IEmailSenderService emailSender,
    IHttpContextAccessor httpContextAccessor) : IAccountService
{
    public async Task<string> ConfirmEmailAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            throw new BadRequestUserException(MessageApi.UserIdTokenRequired);
        }

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new BadRequestUserException(MessageApi.UserNotFound);

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded ? MessageApi.ConfirmingEmail : throw new BadRequestException(MessageApi.ErrorConfirmEmail);
    }

    public async Task<string> ChangeEmailAsync(ChangeEmailModel inputModel)
    {
        if (inputModel.NewEmail == null)
        {
            throw new BadRequestException(MessageApi.NewEmailIsRequired);
        }

        var user = await userManager.FindByEmailAsync(inputModel.Email)
            ?? throw new BadRequestUserException(MessageApi.UserNotFound);

        var userId = await userManager.GetUserIdAsync(user);
        var token = await userManager.GenerateChangeEmailTokenAsync(user, inputModel.NewEmail);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = await GenerateCallBackUrlAsync(userId, token, inputModel.NewEmail);
        await emailSender.SendEmailTypeAsync(inputModel.NewEmail, callbackUrl, EmailSendingType.ChangeEmail);

        return MessageApi.SendEmailForChangeEmail;
    }

    public async Task<string> ConfirmEmailChangeAsync(string userId, string email, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            throw new BadRequestUserException(MessageApi.UserIdEmailTokenRequired);
        }

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new BadRequestUserException(MessageApi.UserNotFound);

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await userManager.ChangeEmailAsync(user, email, code);

        //... Omitted code for username update as in our case username and email are two separate fields.
        //Reference: Lines 57 - 66 in ConfirmEmailChange.cshtml.cs file

        return result.Succeeded ? MessageApi.ConfirmingEmailChanged : throw new BadRequestException(MessageApi.ErrorConfirmEmailChange);
    }

    private async Task<string> GenerateCallBackUrlAsync(string userId, string token, string newEmail)
    {
        var request = httpContextAccessor.HttpContext!.Request;
        var callbackUrl = $"{request.Scheme}://{request.Host}{EndpointsApi.EndpointsAccountGroup}" +
            $"{EndpointsApi.EndpointsConfirmEmailChange}".Replace("{userId}", userId)
            .Replace("{email}", newEmail).Replace("{token}", token);

        return await Task.FromResult(callbackUrl);
    }
}
