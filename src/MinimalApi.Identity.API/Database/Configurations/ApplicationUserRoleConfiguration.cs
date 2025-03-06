using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Database.Configurations;

public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();
    }
}