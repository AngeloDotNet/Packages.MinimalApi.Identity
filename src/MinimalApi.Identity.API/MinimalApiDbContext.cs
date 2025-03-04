using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API;

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

        builder.Entity<ApplicationRole>().HasData(
            new ApplicationRole
            {
                Id = 1,
                Name = nameof(DefaultRoles.Admin),
                NormalizedName = nameof(DefaultRoles.Admin).ToUpper(),
                ConcurrencyStamp = "52D77FEB-3860-4523-B022-4F5CB859E434",
                Default = true
            });

        builder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = nameof(Policy.Licenses), Default = true },
            new Permission { Id = 2, Name = nameof(Policy.Modules), Default = true },
            new Permission { Id = 3, Name = nameof(Policy.Permissions), Default = true },
            new Permission { Id = 4, Name = nameof(Policy.Roles), Default = true },
            new Permission { Id = 5, Name = nameof(Policy.Profiles), Default = true },

            new Permission { Id = 6, Name = nameof(Authorization.GetLicenses), Default = true },
            new Permission { Id = 7, Name = nameof(Authorization.CreateLicense), Default = true },
            new Permission { Id = 8, Name = nameof(Authorization.AssignLicense), Default = true },
            new Permission { Id = 9, Name = nameof(Authorization.DeleteLicense), Default = true },

            new Permission { Id = 10, Name = nameof(Authorization.GetModules), Default = true },
            new Permission { Id = 11, Name = nameof(Authorization.CreateModule), Default = true },
            new Permission { Id = 12, Name = nameof(Authorization.AssignModule), Default = true },
            new Permission { Id = 13, Name = nameof(Authorization.DeleteModule), Default = true },

            new Permission { Id = 14, Name = nameof(Authorization.GetPermissions), Default = true },
            new Permission { Id = 15, Name = nameof(Authorization.CreatePermission), Default = true },
            new Permission { Id = 16, Name = nameof(Authorization.AssignPermission), Default = true },
            new Permission { Id = 17, Name = nameof(Authorization.DeletePermission), Default = true },

            new Permission { Id = 18, Name = nameof(Authorization.GetRoles), Default = true },
            new Permission { Id = 19, Name = nameof(Authorization.CreateRole), Default = true },
            new Permission { Id = 20, Name = nameof(Authorization.AssignRole), Default = true },
            new Permission { Id = 21, Name = nameof(Authorization.DeleteRole), Default = true },

            new Permission { Id = 22, Name = nameof(Authorization.GetProfile), Default = true },
            new Permission { Id = 23, Name = nameof(Authorization.EditProfile), Default = true },
            new Permission { Id = 24, Name = nameof(Authorization.DeleteProfile), Default = true }
        );
    }
}