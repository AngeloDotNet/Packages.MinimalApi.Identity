using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;
using MinimalApi.Identity.API.Exceptions.BadRequest;
using MinimalApi.Identity.API.Exceptions.Conflict;
using MinimalApi.Identity.API.Exceptions.NotFound;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class ClaimsService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : IClaimsService
{
    public async Task<List<ClaimResponseModel>> GetAllClaimsAsync()
    {
        var query = await dbContext.ClaimTypes.AsNoTracking().ToListAsync();

        if (query.Count == 0)
        {
            throw new NotFoundClaimException(MessageApi.ClaimsNotFound);
        }

        return query.Select(c => new ClaimResponseModel(c.Id, c.Type, c.Value, c.Default)).ToList();
    }

    public async Task<string> CreateClaimAsync(CreateClaimModel model)
    {
        if (!CheckClaimTypeIsValid(model.Type))
        {
            throw new BadRequestClaimException(MessageApi.ClaimTypeInvalid);
        }

        if (await CheckClaimExistAsync(model))
        {
            throw new ConflictClaimException(MessageApi.ClaimAlreadyExist);
        }

        var claimType = new ClaimType
        {
            Type = model.Type,
            Value = model.Value,
            Default = false
        };

        dbContext.ClaimTypes.Add(claimType);
        await dbContext.SaveChangesAsync();

        return MessageApi.ClaimCreated;
    }

    public async Task<string> AssignClaimAsync(AssignClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundUserException(MessageApi.UserNotFound);

        var claim = await dbContext.ClaimTypes.AsNoTracking().FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotFoundClaimException(MessageApi.ClaimNotFound);

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            throw new BadRequestClaimException(MessageApi.ClaimAlreadyAssigned);
        }

        var result = await userManager.AddClaimAsync(user, new Claim(model.Type, model.Value));

        return result.Succeeded ? MessageApi.ClaimAssigned : throw new BadRequestClaimException(MessageApi.ClaimNotAssigned);
    }

    public async Task<string> RevokeClaimAsync(RevokeClaimModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString())
            ?? throw new NotFoundUserException(MessageApi.UserNotFound);

        var userHasClaim = await userManager.GetClaimsAsync(user);

        if (!userHasClaim.Any(c => c.Type == model.Type && c.Value == model.Value))
        {
            throw new BadRequestClaimException(MessageApi.ClaimNotAssigned);
        }

        var claimRemove = new Claim(model.Type, model.Value);
        var result = await userManager.RemoveClaimAsync(user, claimRemove);

        return result.Succeeded ? MessageApi.ClaimRevoked : throw new BadRequestClaimException(MessageApi.ClaimNotRevoked);
    }

    public async Task<string> DeleteClaimAsync(DeleteClaimModel model)
    {
        var claim = await dbContext.ClaimTypes.FirstOrDefaultAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotFoundClaimException(MessageApi.ClaimNotFound);

        if (claim.Default)
        {
            throw new BadRequestClaimException(MessageApi.ClaimNotDeleted);
        }

        var isClaimAssigned = await dbContext.Users.AnyAsync(user
            => user.UserClaims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value));

        if (isClaimAssigned)
        {
            throw new BadRequestClaimException(MessageApi.ClaimNotDeleted);
        }

        dbContext.ClaimTypes.Remove(claim);
        await dbContext.SaveChangesAsync();

        return MessageApi.ClaimDeleted;
    }

    private static bool CheckClaimTypeIsValid(string claimType)
        => !string.IsNullOrWhiteSpace(claimType) && Enum.TryParse<ClaimsType>(claimType, true, out _);

    private async Task<bool> CheckClaimExistAsync(CreateClaimModel model)
        => await dbContext.ClaimTypes.AnyAsync(c
            => c.Type.Equals(model.Type, StringComparison.InvariantCultureIgnoreCase)
            && c.Value.Equals(model.Value, StringComparison.InvariantCultureIgnoreCase));
}
