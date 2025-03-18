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
            ConcurrencyStamp = "2F267733-F53E-498F-B91E-C536BCE4AEA3",
            Default = true
        },
        new ApplicationRole
        {
            Id = 2,
            Name = nameof(DefaultRoles.PowerUser),
            NormalizedName = nameof(DefaultRoles.PowerUser).ToUpper(),
            ConcurrencyStamp = "36741E9D-5F55-4994-B9BE-F63F93A81EE0",
            Default = true
        },
        new ApplicationRole
        {
            Id = 3,
            Name = nameof(DefaultRoles.User),
            NormalizedName = nameof(DefaultRoles.User).ToUpper(),
            ConcurrencyStamp = "4535A9B1-B787-4CA6-ACAD-F2E0DF38AB5B",
            Default = true
        });
    }
}