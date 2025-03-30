using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Constants;
using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class LicenseService(MinimalApiAuthDbContext dbContext, UserManager<ApplicationUser> userManager) : ILicenseService
{
    public async Task<IResult> GetAllLicensesAsync()
    {
        var licenses = await dbContext.Licenses
            .Select(l => new LicenseResponseModel(l.Id, l.Name, l.ExpirationDate))
            .ToListAsync();

        return licenses.Count == 0 ? TypedResults.NotFound(MessageApi.LicensesNotFound) : TypedResults.Ok(licenses);
    }

    public async Task<IResult> CreateLicenseAsync(CreateLicenseModel model)
    {
        if (await CheckLicenseExistAsync(model))
        {
            return TypedResults.Conflict(MessageApi.LicenseAlreadyExist);
        }

        var license = new License
        {
            Name = model.Name,
            ExpirationDate = model.ExpirationDate
        };

        dbContext.Licenses.Add(license);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.LicenseCreated);
    }

    public async Task<IResult> AssignLicenseAsync(AssignLicenseModel model)
    {
        var user = await userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
        {
            return TypedResults.NotFound(MessageApi.UserNotFound);
        }

        var license = await dbContext.Licenses.FindAsync(model.LicenseId);
        if (license == null)
        {
            return TypedResults.NotFound(MessageApi.LicenseNotFound);
        }

        var userHasLicense = await dbContext.UserLicenses
            .AnyAsync(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId);

        if (userHasLicense)
        {
            return TypedResults.BadRequest(MessageApi.LicenseNotAssignable);
        }

        var userLicense = new UserLicense
        {
            UserId = model.UserId,
            LicenseId = model.LicenseId
        };

        dbContext.UserLicenses.Add(userLicense);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.LicenseAssigned);
    }

    public async Task<IResult> RevokeLicenseAsync(RevokeLicenseModel model)
    {
        var userLicense = await dbContext.UserLicenses
            .SingleOrDefaultAsync(ul => ul.UserId == model.UserId && ul.LicenseId == model.LicenseId);

        if (userLicense == null)
        {
            return TypedResults.NotFound(MessageApi.LicenseNotFound);
        }

        dbContext.UserLicenses.Remove(userLicense);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.LicenseCanceled);
    }

    public async Task<IResult> DeleteLicenseAsync(DeleteLicenseModel model)
    {
        var license = await dbContext.Licenses.FindAsync(model.LicenseId);
        if (license == null)
        {
            return TypedResults.NotFound(MessageApi.LicenseNotFound);
        }

        if (await dbContext.UserLicenses.AnyAsync(ul => ul.LicenseId == model.LicenseId))
        {
            return TypedResults.BadRequest(MessageApi.LicenseNotDeleted);
        }

        dbContext.Licenses.Remove(license);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(MessageApi.LicenseDeleted);
    }

    public async Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user)
    {
        var result = await dbContext.UserLicenses
            .AsNoTracking()
            .Include(ul => ul.License)
            .FirstOrDefaultAsync(ul => ul.UserId == user.Id);

        return result != null ? new Claim(CustomClaimTypes.License, result.License.Name) : null!;
    }

    public async Task<bool> CheckUserLicenseExpiredAsync(ApplicationUser user)
    {
        return await dbContext.UserLicenses
            .AsNoTracking()
            .Include(ul => ul.License)
            .AnyAsync(ul => ul.UserId == user.Id && ul.License.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow));
    }

    private async Task<bool> CheckLicenseExistAsync(CreateLicenseModel model)
    {
        return await dbContext.Licenses.AnyAsync(l => l.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase));
    }
}
