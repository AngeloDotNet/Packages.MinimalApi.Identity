using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Database.Configurations;

public class ClaimTypeConfiguration : IEntityTypeConfiguration<ClaimType>
{
    public void Configure(EntityTypeBuilder<ClaimType> builder)
    {
        builder.HasData(new ClaimType
        {
            Id = 1,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.AuthPolicy),
            Default = true
        },
        new ClaimType
        {
            Id = 2,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.AuthPolicyRead),
            Default = true
        },
        new ClaimType
        {
            Id = 3,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.AuthPolicyWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 4,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.Claim),
            Default = true
        },
        new ClaimType
        {
            Id = 5,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.ClaimRead),
            Default = true
        },
        new ClaimType
        {
            Id = 6,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.ClaimWrite),
            Default = true
        },

        new ClaimType
        {
            Id = 7,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.Licenza),
            Default = true
        },
        new ClaimType
        {
            Id = 8,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.LicenzaRead),
            Default = true
        },
        new ClaimType
        {
            Id = 9,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.LicenzaWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 10,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.Modulo),
            Default = true
        },
        new ClaimType
        {
            Id = 11,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.ModuloRead),
            Default = true
        },
        new ClaimType
        {
            Id = 12,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.ModuloWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 13,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.Profilo),
            Default = true
        },
        new ClaimType
        {
            Id = 14,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.ProfiloRead),
            Default = true
        },
        new ClaimType
        {
            Id = 15,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.ProfiloWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 16,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.Ruolo),
            Default = true
        },
        new ClaimType
        {
            Id = 17,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.RuoloRead),
            Default = true
        },
        new ClaimType
        {
            Id = 18,
            Type = nameof(ClaimsType.Permission),
            Value = nameof(Permissions.RuoloWrite),
            Default = true
        });
    }
}