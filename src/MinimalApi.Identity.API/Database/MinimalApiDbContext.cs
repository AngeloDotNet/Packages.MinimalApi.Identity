using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Database;

public class MinimalApiDbContext(DbContextOptions<MinimalApiDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, int,
    IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    public DbSet<License> Licenses { get; set; }
    public DbSet<UserLicense> UserLicenses { get; set; }
    public DbSet<Entities.Module> Modules { get; set; }
    public DbSet<UserModule> UserModules { get; set; }
    public DbSet<EmailSending> EmailSendings { get; set; }
    public DbSet<EmailSendingType> EmailSendingTypes { get; set; }
    public DbSet<ClaimType> ClaimTypes { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}