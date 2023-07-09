using FAPIServer.Storage.EntityFramework.Entities.Converters;
using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.GrantsContext.Config;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(p => p.Token);

        builder.HasIndex(p => p.Subject);
        builder.HasIndex(p => p.ClientId);

        builder.Property(p => p.AuthorizationDetails)
            .HasConversion<AuthorizationDetailsEnumerableConverter>();

        builder.Property(p => p.Claims)
            .HasConversion<StringEnumerableConverter>();

        builder.HasOne(p => p.Grant)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
