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

        builder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Licenses", Default = true },
            new Permission { Id = 2, Name = "Modules", Default = true },
            new Permission { Id = 3, Name = "Permissions", Default = true },
            new Permission { Id = 4, Name = "Roles", Default = true },
            new Permission { Id = 5, Name = "Users", Default = true },

            new Permission { Id = 6, Name = "GetLicenses", Default = true },
            new Permission { Id = 7, Name = "CreateLicenses", Default = true },
            new Permission { Id = 8, Name = "AssignLicense", Default = true },
            new Permission { Id = 9, Name = "DeleteLicenses", Default = true },

            new Permission { Id = 10, Name = "GetModules", Default = true },
            new Permission { Id = 11, Name = "CreateModules", Default = true },
            new Permission { Id = 12, Name = "AssignModule", Default = true },
            new Permission { Id = 13, Name = "DeleteModules", Default = true },

            new Permission { Id = 14, Name = "GetPermissions", Default = true },
            new Permission { Id = 15, Name = "CreatePermissions", Default = true },
            new Permission { Id = 16, Name = "AssignPermission", Default = true },
            new Permission { Id = 17, Name = "DeletePermissions", Default = true },

            new Permission { Id = 18, Name = "GetRoles", Default = true },
            new Permission { Id = 19, Name = "CreateRoles", Default = true },
            new Permission { Id = 20, Name = "AssignRole", Default = true },
            new Permission { Id = 21, Name = "DeleteRoles", Default = true }
        );
    }
}