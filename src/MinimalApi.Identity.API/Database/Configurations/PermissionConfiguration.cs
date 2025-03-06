using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Database.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasData(new Permission
        {
            Id = 1,
            Name = nameof(Policy.Licenses),
            Default = true
        },
        new Permission
        {
            Id = 2,
            Name = nameof(Policy.Modules),
            Default = true
        },
        new Permission
        {
            Id = 3,
            Name = nameof(Policy.Permissions),
            Default = true
        },
        new Permission
        {
            Id = 4,
            Name = nameof(Policy.Roles),
            Default = true
        },
        new Permission
        {
            Id = 5,
            Name = nameof(Policy.Profiles),
            Default = true
        },
        new Permission
        {
            Id = 6,
            Name = nameof(Authorize.GetLicenses),
            Default = true
        },
        new Permission
        {
            Id = 7,
            Name = nameof(Authorize.CreateLicense),
            Default = true
        },
        new Permission
        {
            Id = 8,
            Name = nameof(Authorize.AssignLicense),
            Default = true
        },
        new Permission
        {
            Id = 9,
            Name = nameof(Authorize.DeleteLicense),
            Default = true
        },
        new Permission
        {
            Id = 10,
            Name = nameof(Authorize.GetModules),
            Default = true
        },
        new Permission
        {
            Id = 11,
            Name = nameof(Authorize.CreateModule),
            Default = true
        },
        new Permission
        {
            Id = 12,
            Name = nameof(Authorize.AssignModule),
            Default = true
        },
        new Permission
        {
            Id = 13,
            Name = nameof(Authorize.DeleteModule),
            Default = true
        },
        new Permission
        {
            Id = 14,
            Name = nameof(Authorize.GetPermissions),
            Default = true
        },
        new Permission
        {
            Id = 15,
            Name = nameof(Authorize.CreatePermission),
            Default = true
        },
        new Permission
        {
            Id = 16,
            Name = nameof(Authorize.AssignPermission),
            Default = true
        },
        new Permission
        {
            Id = 17,
            Name = nameof(Authorize.DeletePermission),
            Default = true
        },
        new Permission
        {
            Id = 18,
            Name = nameof(Authorize.GetRoles),
            Default = true
        },
        new Permission
        {
            Id = 19,
            Name = nameof(Authorize.CreateRole),
            Default = true
        },
        new Permission
        {
            Id = 20,
            Name = nameof(Authorize.AssignRole),
            Default = true
        },
        new Permission
        {
            Id = 21,
            Name = nameof(Authorize.DeleteRole),
            Default = true
        },
        new Permission
        {
            Id = 22,
            Name = nameof(Authorize.GetProfile),
            Default = true
        },
        new Permission
        {
            Id = 23,
            Name = nameof(Authorize.EditProfile),
            Default = true
        },
        new Permission
        {
            Id = 24,
            Name = nameof(Authorize.DeleteProfile),
            Default = true
        });
    }
}