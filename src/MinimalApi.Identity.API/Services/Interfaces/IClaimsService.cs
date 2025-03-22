using Microsoft.AspNetCore.Http;
using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IClaimsService
{
    Task<IResult> GetAllClaimsAsync();
    Task<IResult> CreateClaimAsync(CreateClaimModel model);
    Task<IResult> AssignClaimAsync(AssignClaimModel model);
    Task<IResult> RevokeClaimAsync(RevokeClaimModel model);
    Task<IResult> DeleteClaimAsync(DeleteClaimModel model);
}