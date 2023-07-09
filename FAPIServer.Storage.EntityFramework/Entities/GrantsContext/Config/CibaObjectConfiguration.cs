using FAPIServer.Storage.EntityFramework.Entities.Converters;
using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.GrantsContext.Config;

public class CibaObjectConfiguration : IEntityTypeConfiguration<CibaObject>
{
    public void Configure(EntityTypeBuilder<CibaObject> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Subject);
        builder.HasIndex(p => p.ClientId);

        builder.Property(p => p.AuthorizationDetails)
            .HasConversion<AuthorizationDetailsEnumerableConverter>();

        builder.Property(p => p.Claims)
            .HasConversion<StringEnumerableConverter>();

        builder.HasOne(p => p.Grant)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.DPoPPkh)
            .HasConversion<Base64UrlEncodedStringConverter>();
    }
}
