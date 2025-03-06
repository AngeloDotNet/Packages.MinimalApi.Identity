using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AccountService(UserManager<ApplicationUser> userManager, IEmailSenderService emailSender,
    IHttpContextAccessor httpContextAccessor) : IAccountService
{
    public async Task<Results<Ok<string>, BadRequest<string>>> ConfirmEmailAsync(string userId, string token)
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

    public async Task<Results<Ok<string>, BadRequest<string>>> ChangeEmailAsync(ChangeEmailModel inputModel)
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

        var request = httpContextAccessor.HttpContext!.Request;
        var callbackUrl = $"{request.Scheme}://{request.Host}{EndpointsApi.EndpointsAccountGroup}" +
        $"{EndpointsApi.EndpointsConfirmEmailChange}".Replace("{userId}", userId).Replace("{email}", inputModel.NewEmail)
        .Replace("{token}", token);

        await emailSender.SendEmailAsync(inputModel.NewEmail!, "Confirm your email",
            $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.", 2);

        return TypedResults.Ok(MessageApi.SendEmailForChangeEmail);
    }

    public async Task<Results<Ok<string>, BadRequest<string>>> ConfirmEmailChangeAsync(string userId, string email, string token)
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

    //public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    //{
    //    return await userManager.FindByEmailAsync(email);
    //}
    //public async Task<ApplicationUser> GetUserByIdAsync(string userId)
    //{
    //    return await userManager.FindByIdAsync(userId);
    //}
    //public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
    //{
    //    return await userManager.FindByNameAsync(userName);
    //}
    //public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
    //{
    //    return await userManager.IsEmailConfirmedAsync(user);
    //}
    //public async Task<bool> IsUserLockedOutAsync(ApplicationUser user)
    //{
    //    return await userManager.IsLockedOutAsync(user);
    //}
    //public async Task<bool> IsUserValidAsync(ApplicationUser user, string password)
    //{
    //    return await userManager.CheckPasswordAsync(user, password);
    //}
}