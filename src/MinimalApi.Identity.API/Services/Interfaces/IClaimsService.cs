using MinimalApi.Identity.API.Models;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IClaimsService
{
    Task<List<ClaimResponseModel>> GetAllClaimsAsync();
    Task<string> CreateClaimAsync(CreateClaimModel model);
    Task<string> AssignClaimAsync(AssignClaimModel model);
    Task<string> RevokeClaimAsync(RevokeClaimModel model);
    Task<string> DeleteClaimAsync(DeleteClaimModel model);
}