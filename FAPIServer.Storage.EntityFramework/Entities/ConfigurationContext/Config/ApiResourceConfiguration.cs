using FAPIServer.Storage.EntityFramework.Entities.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext.Config;

public class ApiResourceConfiguration : IEntityTypeConfiguration<ApiResource>
{
    public void Configure(EntityTypeBuilder<ApiResource> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Uri).IsUnique();

        builder.Property(prop => prop.Uri)
             .HasConversion<UriConverter>();
    }
}
