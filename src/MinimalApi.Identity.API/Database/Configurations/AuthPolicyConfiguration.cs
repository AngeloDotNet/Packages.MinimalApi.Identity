using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Database.Configurations;

public class AuthPolicyConfiguration : IEntityTypeConfiguration<AuthPolicy>
{
    public void Configure(EntityTypeBuilder<AuthPolicy> builder)
    {
        builder.HasData(new AuthPolicy
        {
            Id = 1,
            PolicyName = nameof(Permissions.AuthPolicyRead),
            PolicyDescription = "AuthPolicyRead",
            PolicyPermissions = [nameof(Permissions.AuthPolicy), nameof(Permissions.AuthPolicyRead)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 2,
            PolicyName = nameof(Permissions.AuthPolicyWrite),
            PolicyDescription = "AuthPolicyWrite",
            PolicyPermissions = [nameof(Permissions.AuthPolicy), nameof(Permissions.AuthPolicyWrite)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 3,
            PolicyName = nameof(Permissions.ClaimRead),
            PolicyDescription = "ClaimRead",
            PolicyPermissions = [nameof(Permissions.Claim), nameof(Permissions.ClaimRead)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 4,
            PolicyName = nameof(Permissions.ClaimWrite),
            PolicyDescription = "ClaimWrite",
            PolicyPermissions = [nameof(Permissions.Claim), nameof(Permissions.ClaimWrite)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 5,
            PolicyName = nameof(Permissions.LicenzaRead),
            PolicyDescription = "LicenzaRead",
            PolicyPermissions = [nameof(Permissions.Licenza), nameof(Permissions.LicenzaRead)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 6,
            PolicyName = nameof(Permissions.LicenzaWrite),
            PolicyDescription = "LicenzaWrite",
            PolicyPermissions = [nameof(Permissions.Licenza), nameof(Permissions.LicenzaWrite)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 7,
            PolicyName = nameof(Permissions.ModuloRead),
            PolicyDescription = "ModuloRead",
            PolicyPermissions = [nameof(Permissions.Modulo), nameof(Permissions.ModuloRead)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 8,
            PolicyName = nameof(Permissions.ModuloWrite),
            PolicyDescription = "ModuloWrite",
            PolicyPermissions = [nameof(Permissions.Modulo), nameof(Permissions.ModuloWrite)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 9,
            PolicyName = nameof(Permissions.ProfiloRead),
            PolicyDescription = "ProfiloRead",
            PolicyPermissions = [nameof(Permissions.Profilo), nameof(Permissions.ProfiloRead)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 10,
            PolicyName = nameof(Permissions.ProfiloWrite),
            PolicyDescription = "ProfiloWrite",
            PolicyPermissions = [nameof(Permissions.Profilo), nameof(Permissions.ProfiloWrite)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 11,
            PolicyName = nameof(Permissions.RuoloRead),
            PolicyDescription = "RuoloRead",
            PolicyPermissions = [nameof(Permissions.Ruolo), nameof(Permissions.RuoloRead)],
            IsDefault = true,
            IsActive = true
        },
        new AuthPolicy
        {
            Id = 12,
            PolicyName = nameof(Permissions.RuoloWrite),
            PolicyDescription = "RuoloWrite",
            PolicyPermissions = [nameof(Permissions.Ruolo), nameof(Permissions.RuoloWrite)],
            IsDefault = true,
            IsActive = true
        });
    }
}