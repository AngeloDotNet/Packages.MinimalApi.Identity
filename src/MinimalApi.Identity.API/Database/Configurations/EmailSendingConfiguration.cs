using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalApi.Identity.API.Entities;

namespace MinimalApi.Identity.API.Database.Configurations;

public class EmailSendingConfiguration : IEntityTypeConfiguration<EmailSending>
{
    public void Configure(EntityTypeBuilder<EmailSending> builder)
    {
        builder.Property(x => x.EmailSendingType).HasConversion<string>();
    }
}
