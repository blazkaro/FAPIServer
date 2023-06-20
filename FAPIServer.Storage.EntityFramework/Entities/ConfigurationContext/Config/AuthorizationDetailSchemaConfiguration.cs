using FAPIServer.Storage.EntityFramework.Entities.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext.Config;

public class AuthorizationDetailSchemaConfiguration : IEntityTypeConfiguration<AuthorizationDetailSchema>
{
    public void Configure(EntityTypeBuilder<AuthorizationDetailSchema> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Type).IsUnique();

        builder.Property(p => p.DefaultActionSchema)
            .HasConversion<JsonSchemaConverter>();

        builder.Property(p => p.ExtensionsSchema)
            .HasConversion<JsonSchemaConverter>();

        builder.HasData(new AuthorizationDetailSchema
        {
            Id = 1,
            Type = "openid",
            Enabled = true,
            DefaultActionSchema = null,
            ExtensionsSchema = null,
            IsReusable = true,
            ShowInDiscoveryDocument = true,
            DisplayName = "OpenID",
            Description = "Access to OpenID data"
        });
    }
}
