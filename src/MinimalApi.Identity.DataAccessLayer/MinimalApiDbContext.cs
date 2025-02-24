using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.DataAccessLayer.Entities;
using MinimalApi.Identity.Shared;

namespace MinimalApi.Identity.DataAccessLayer;

public class MinimalApiDbContext(DbContextOptions<MinimalApiDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, int,
    IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<License> Licenses { get; set; }
    public DbSet<UserLicense> UserLicenses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<UserModule> UserModules { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Entity<ApplicationUserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId).IsRequired();

        builder.Entity<ApplicationUserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId).IsRequired();

        builder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        builder.Entity<UserLicense>().HasKey(ul => new { ul.UserId, ul.LicenseId });

        builder.Entity<UserLicense>()
            .HasOne(ul => ul.User)
            .WithMany(u => u.UserLicenses)
            .HasForeignKey(ul => ul.UserId);

        builder.Entity<UserLicense>()
            .HasOne(ul => ul.License)
            .WithMany(l => l.UserLicenses)
            .HasForeignKey(ul => ul.LicenseId);

        builder.Entity<UserModule>().HasKey(um => new { um.UserId, um.ModuleId });

        builder.Entity<UserModule>()
            .HasOne(um => um.User)
            .WithMany(u => u.UserModules)
            .HasForeignKey(um => um.UserId);

        builder.Entity<UserModule>()
            .HasOne(um => um.Module)
            .WithMany(m => m.UserModules)
            .HasForeignKey(um => um.ModuleId);
    }
}
