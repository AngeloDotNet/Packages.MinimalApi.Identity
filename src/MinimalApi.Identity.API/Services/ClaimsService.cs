using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ClaimsService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IClaimsService
{
    public async Task<IResult> GetAllClaimsAsync()
    {
        var query = await dbContext.ClaimTypes.AsNoTracking().ToListAsync();

        if (query == null || query.Count == 0)
        {
            return TypedResults.NotFound(MessageApi.ClaimsNotFound);
        }

        var result = query.Select(c => new ClaimResponseModel(c.Id, c.Type, c.Value, c.Default)).ToList();

        return TypedResults.Ok(result);
    }

    public async Task<IResult> CreateClaimAsync(CreateClaimModel model)
    {
        if (!CheckClaimTypeIsValid(model.Type))
        {
            return TypedResults.BadRequest(MessageApi.ClaimTypeInvalid);
        }

        var claimType = new ClaimType
        {
            Type = model.Type,
            Value = model.Value,
            Default = false
        };

        if (await CheckClaimExistAsync(model))
        {
            return TypedResults.Conflict(MessageApi.ClaimAlreadyExist);
        }

        dbContext.ClaimTypes.Add(claimType);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.ClaimCreated);
    }

    public async Task<IResult> AssignClaimAsync(AssignClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.UserNotFound);
        }

        var claim = await dbContext.ClaimTypes.AsNoTracking().FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase));

        if (claim == null)
        {
            return TypedResults.NotFound(MessageApi.ClaimNotFound);
        }

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            return TypedResults.BadRequest(MessageApi.ClaimAlreadyAssigned);
        }

        var result = await userManager.AddClaimAsync(user, new Claim(model.Type, model.Value));

        return result.Succeeded ? TypedResults.Ok(MessageApi.ClaimAssigned) : TypedResults.BadRequest(MessageApi.ClaimNotAssigned);
    }

    public async Task<IResult> RevokeClaimAsync(RevokeClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.UserNotFound);
        }

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (!userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            return TypedResults.BadRequest(MessageApi.ClaimNotAssigned);
        }

        var claimRemove = new Claim(model.Type, model.Value);
        var result = await userManager.RemoveClaimAsync(user, claimRemove);

        return result.Succeeded ? TypedResults.Ok(MessageApi.ClaimRevoked) : TypedResults.BadRequest(MessageApi.ClaimNotRevoked);
    }

    public async Task<IResult> DeleteClaimAsync(DeleteClaimModel model)
    {
        var claim = await dbContext.ClaimTypes.FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase));

        if (claim == null)
        {
            return TypedResults.NotFound(MessageApi.ClaimNotFound);
        }

        if (claim.Default)
        {
            return TypedResults.BadRequest(MessageApi.ClaimNotDeleted);
        }

        var isClaimAssigned = await dbContext.Users.AnyAsync(user
            => user.UserClaims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value));

        if (isClaimAssigned)
        {
            return TypedResults.BadRequest(MessageApi.ClaimNotDeleted);
        }

        dbContext.ClaimTypes.Remove(claim);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.ClaimDeleted);
    }

    private static bool CheckClaimTypeIsValid(string claimType)
        => !string.IsNullOrWhiteSpace(claimType) && Enum.TryParse<ClaimsType>(claimType, true, out _);

    private async Task<bool> CheckClaimExistAsync(CreateClaimModel model)
        => await dbContext.ClaimTypes.AnyAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase));
}