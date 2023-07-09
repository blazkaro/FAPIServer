using FAPIServer.Storage.EntityFramework.Entities.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext.Config;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.ClientId).IsUnique();

        builder.Property(p => p.Ed25519PublicKey)
            .HasConversion<Ed25519KeyConverter>();

        builder.Property(p => p.BackchannelClientNotificationEndpoint)
            .HasConversion<UriConverter>();

        builder.Property(p => p.RequestUriLifetime)
            .HasConversion<LifetimeConverter>();

        builder.Property(p => p.AuthorizationCodeLifetime)
            .HasConversion<LifetimeConverter>();

        builder.Property(p => p.IdTokenLifetime)
            .HasConversion<LifetimeConverter>();

        builder.Property(p => p.AccessTokenLifetime)
            .HasConversion<LifetimeConverter>();

        builder.Property(p => p.RefreshTokenLifetime)
            .HasConversion<LifetimeConverter>();

        builder.Property(p => p.CibaRequestLifetime)
            .HasConversion<LifetimeConverter>();

        builder.Property(p => p.LogoUri)
            .HasConversion<UriConverter>();
    }
}
