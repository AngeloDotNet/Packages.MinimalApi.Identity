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
            Type = "Permission",
            Value = nameof(Permissions.Profilo),
            Default = true
        },
        new ClaimType
        {
            Id = 2,
            Type = "Permission",
            Value = nameof(Permissions.ProfiloRead),
            Default = true
        },
        new ClaimType
        {
            Id = 3,
            Type = "Permission",
            Value = nameof(Permissions.ProfiloWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 4,
            Type = "Permission",
            Value = nameof(Permissions.Ruolo),
            Default = true
        },
        new ClaimType
        {
            Id = 5,
            Type = "Permission",
            Value = nameof(Permissions.RuoloRead),
            Default = true
        },
        new ClaimType
        {
            Id = 6,
            Type = "Permission",
            Value = nameof(Permissions.RuoloWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 7,
            Type = "Permission",
            Value = nameof(Permissions.Permesso),
            Default = true
        },
        new ClaimType
        {
            Id = 8,
            Type = "Permission",
            Value = nameof(Permissions.PermessoRead),
            Default = true
        },
        new ClaimType
        {
            Id = 9,
            Type = "Permission",
            Value = nameof(Permissions.PermessoWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 10,
            Type = "Permission",
            Value = nameof(Permissions.Modulo),
            Default = true
        },
        new ClaimType
        {
            Id = 11,
            Type = "Permission",
            Value = nameof(Permissions.ModuloRead),
            Default = true
        },
        new ClaimType
        {
            Id = 12,
            Type = "Permission",
            Value = nameof(Permissions.ModuloWrite),
            Default = true
        },
        new ClaimType
        {
            Id = 13,
            Type = "Permission",
            Value = nameof(Permissions.Licenza),
            Default = true
        },
        new ClaimType
        {
            Id = 14,
            Type = "Permission",
            Value = nameof(Permissions.LicenzaRead),
            Default = true
        },
        new ClaimType
        {
            Id = 15,
            Type = "Permission",
            Value = nameof(Permissions.LicenzaWrite),
            Default = true
        }
        );
    }
}