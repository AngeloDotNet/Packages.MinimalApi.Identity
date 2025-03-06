using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Database.Configurations;

public class UserModuleConfiguration : IEntityTypeConfiguration<UserModule>
{
    public void Configure(EntityTypeBuilder<UserModule> builder)
    {
        builder.HasKey(um => new { um.UserId, um.ModuleId });

        builder.HasOne(um => um.User)
            .WithMany(u => u.UserModules).HasForeignKey(um => um.UserId).IsRequired();

        builder.HasOne(um => um.Module)
            .WithMany(m => m.UserModules).HasForeignKey(um => um.ModuleId).IsRequired();
    }
}
