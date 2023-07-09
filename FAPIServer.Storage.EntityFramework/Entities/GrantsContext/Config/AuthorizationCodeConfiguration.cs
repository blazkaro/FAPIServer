using FAPIServer.Storage.EntityFramework.Entities.Converters;
using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.GrantsContext.Config;

public class AuthorizationCodeConfiguration : IEntityTypeConfiguration<AuthorizationCode>
{
    public void Configure(EntityTypeBuilder<AuthorizationCode> builder)
    {
        builder.HasKey(p => p.Code);

        builder.HasIndex(p => p.Subject);
        builder.HasIndex(p => p.ClientId);

        builder.Property(p => p.AuthorizationDetails)
            .HasConversion<AuthorizationDetailsEnumerableConverter>();

        builder.Property(p => p.Claims)
            .HasConversion<StringEnumerableConverter>();

        builder.Property(p => p.CodeChallenge)
            .HasConversion<Base64UrlEncodedStringConverter>();

        builder.Property(p => p.DPoPPkh)
            .HasConversion<Base64UrlEncodedStringConverter>();

        builder.HasOne(p => p.Grant)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
