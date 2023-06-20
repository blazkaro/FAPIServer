using FAPIServer.Storage.EntityFramework.Entities.Converters;
using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.GrantsContext.Config;

public class ParObjectConfiguration : IEntityTypeConfiguration<ParObject>
{
    public void Configure(EntityTypeBuilder<ParObject> builder)
    {
        builder.HasKey(p => p.Uri);

        builder.HasIndex(p => p.ClientId);

        builder.Property(p => p.AuthorizationDetails)
            .HasConversion<AuthorizationDetailsEnumerableConverter>();

        builder.Property(p => p.Claims)
            .HasConversion<StringEnumerableConverter>();

        builder.Property(p => p.CodeChallenge)
            .HasConversion<Base64UrlEncodedStringConverter>();

        builder.Property(p => p.DPoPPkh)
            .HasConversion<Base64UrlEncodedStringConverter>();
    }
}
