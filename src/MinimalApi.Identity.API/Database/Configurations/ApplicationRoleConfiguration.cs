using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Database.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(new ApplicationRole
        {
            Id = 1,
            Name = nameof(DefaultRoles.Admin),
            NormalizedName = nameof(DefaultRoles.Admin).ToUpper(),
            ConcurrencyStamp = "52D77FEB-3860-4523-B022-4F5CB859E434",
            Default = true
        });
    }
}