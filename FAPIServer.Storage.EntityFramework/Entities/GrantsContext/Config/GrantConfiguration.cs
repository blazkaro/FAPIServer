using FAPIServer.Storage.EntityFramework.Entities.Converters;
using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.GrantsContext.Config;

public class GrantConfiguration : IEntityTypeConfiguration<Grant>
{
    public void Configure(EntityTypeBuilder<Grant> builder)
    {
        builder.HasKey(p => p.GrantId);

        builder.HasIndex(p => p.Subject);
        builder.HasIndex(p => p.ClientId);

        builder.Property(p => p.AuthorizationDetails)
            .HasConversion<AuthorizationDetailsCollectionConverter>();

        builder.Property(p => p.Claims)
            .HasConversion<StringCollectionConverter>();
    }
}
