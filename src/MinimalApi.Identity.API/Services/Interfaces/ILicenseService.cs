using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface ILicenseService
{
    Task<IResult> GetAllLicensesAsync();
    Task<IResult> CreateLicenseAsync(CreateLicenseModel model);
    Task<IResult> AssignLicenseAsync(AssignLicenseModel model);
    Task<IResult> RevokeLicenseAsync(RevokeLicenseModel model);
}