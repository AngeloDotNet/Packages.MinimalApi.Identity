using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AccountService(UserManager<ApplicationUser> userManager, IEmailSenderService emailSender,
    IHttpContextAccessor httpContextAccessor) : IAccountService
{
    public async Task<IResult> ConfirmEmailAsync(string userId, string token)
    {
        if (userId == null || token == null)
        {
            return TypedResults.BadRequest(MessageApi.UserIdTokenRequired);
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return TypedResults.BadRequest(MessageApi.UserNotFound);
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded ? TypedResults.Ok(MessageApi.ConfirmingEmail) : TypedResults.BadRequest(MessageApi.ErrorConfirmEmail);
    }

    public async Task<IResult> ChangeEmailAsync(ChangeEmailModel inputModel)
    {
        var user = await userManager.FindByEmailAsync(inputModel.Email);

        if (user == null)
        {
            return TypedResults.BadRequest(MessageApi.UserNotFound);
        }

        if (inputModel.NewEmail == null)
        {
            return TypedResults.BadRequest(MessageApi.EmailUnchanged);
        }

        var userId = await userManager.GetUserIdAsync(user);
        var token = await userManager.GenerateChangeEmailTokenAsync(user, inputModel.Email);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = await GenerateCallBackUrlAsync(userId, token, inputModel.NewEmail);

        await emailSender.SendEmailTypeAsync(inputModel.NewEmail!, callbackUrl, EmailSendingType.ChangeEmail);

        return TypedResults.Ok(MessageApi.SendEmailForChangeEmail);
    }

    public async Task<IResult> ConfirmEmailChangeAsync(string userId, string email, string token)
    {
        if (userId == null || email == null || token == null)
        {
            return TypedResults.BadRequest(MessageApi.UserIdEmailTokenRequired);
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return TypedResults.BadRequest(MessageApi.UserNotFound);
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await userManager.ChangeEmailAsync(user, email, code);

        //... Omitted code for username update as in our case username and email are two separate fields.
        //Reference: Lines 57 - 66 in ConfirmEmailChange.cshtml.cs file

        return result.Succeeded ? TypedResults.Ok(MessageApi.ConfirmingEmailChanged) : TypedResults.BadRequest(MessageApi.ErrorConfirmEmailChange);
    }

    private async Task<string> GenerateCallBackUrlAsync(string userId, string token, string newEmail)
    {
        var request = httpContextAccessor.HttpContext!.Request;

        var callbackUrl = $"{request.Scheme}://{request.Host}{EndpointsApi.EndpointsAccountGroup}{EndpointsApi.EndpointsConfirmEmailChange}"
            .Replace("{userId}", userId)
            .Replace("{email}", newEmail).Replace("{token}", token);

        await Task.Delay(500);

        return callbackUrl;
    }
}