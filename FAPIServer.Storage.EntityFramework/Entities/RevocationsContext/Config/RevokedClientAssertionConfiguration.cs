using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.RevocationsContext.Config;

public class RevokedClientAssertionConfiguration : IEntityTypeConfiguration<RevokedClientAssertion>
{
    public void Configure(EntityTypeBuilder<RevokedClientAssertion> builder)
    {
        builder.HasKey(p => new { p.Jti, p.ClientId });
    }
}
