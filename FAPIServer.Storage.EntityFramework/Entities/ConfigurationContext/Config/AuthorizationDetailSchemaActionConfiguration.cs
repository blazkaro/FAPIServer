using FAPIServer.Storage.EntityFramework.Entities.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext.Config;

public class AuthorizationDetailSchemaActionConfiguration : IEntityTypeConfiguration<AuthorizationDetailSchemaAction>
{
    public void Configure(EntityTypeBuilder<AuthorizationDetailSchemaAction> builder)
    {
        builder.HasKey(k => new { k.Name, k.AuthorizationDetailSchemaId });

        builder.Property(p => p.ActionSchema)
            .HasConversion<JsonSchemaConverter>();

        builder.HasData(
            new AuthorizationDetailSchemaAction
            {
                Name = "offline_access",
                Enabled = true,
                AuthorizationDetailSchemaId = 1,
                ActionSchema = null,
                UseDefaultSchema = false,
                IsEnriched = false,
                DisplayName = "Offline Access",
                Description = "Offline access to your data"
            },
            new AuthorizationDetailSchemaAction
            {
                Name = "grant_management_query",
                Enabled = true,
                AuthorizationDetailSchemaId = 1,
                ActionSchema = null,
                UseDefaultSchema = false,
                IsEnriched = false,
                DisplayName = "Grant Querying",
                Description = "Query your consent information"
            },
            new AuthorizationDetailSchemaAction
            {
                Name = "grant_management_revoke",
                Enabled = true,
                AuthorizationDetailSchemaId = 1,
                ActionSchema = null,
                UseDefaultSchema = false,
                IsEnriched = false,
                DisplayName = "Grant Revocation",
                Description = "Revoke your consent"
            });
    }
}
