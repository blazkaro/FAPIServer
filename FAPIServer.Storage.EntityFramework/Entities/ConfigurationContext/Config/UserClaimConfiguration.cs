using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext.Config;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Type).IsUnique();

        builder.HasData(new UserClaim
        {
            Id = 1,
            Enabled = true,
            Type = "sub",
            ShowInDiscoveryDocument = true,
            Description = "The user identifier"
        });
    }
}
