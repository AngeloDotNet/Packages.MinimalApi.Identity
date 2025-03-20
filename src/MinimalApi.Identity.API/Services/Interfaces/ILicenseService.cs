using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface ILicenseService
{
    Task<IResult> GetAllLicensesAsync();
    Task<IResult> CreateLicenseAsync(CreateLicenseModel model);
    Task<IResult> AssignLicenseAsync(AssignLicenseModel model);
    Task<IResult> RevokeLicenseAsync(RevokeLicenseModel model);
    Task<IResult> DeleteLicenseAsync(DeleteLicenseModel model);

    Task<Claim> GetClaimLicenseUserAsync(ApplicationUser user);
}