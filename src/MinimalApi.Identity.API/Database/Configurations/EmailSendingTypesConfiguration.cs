using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Database.Configurations;

public class EmailSendingTypesConfiguration : IEntityTypeConfiguration<EmailSendingType>
{
    public void Configure(EntityTypeBuilder<EmailSendingType> builder)
    {
        builder.HasData(new EmailSendingType
        {
            Id = 1,
            EmailType = "RegisterUser"
        },
        new EmailSendingType
        {
            Id = 2,
            EmailType = "ChangeEmail"
        });
    }
}