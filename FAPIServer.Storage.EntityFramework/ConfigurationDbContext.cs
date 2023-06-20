using FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;
using FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext.Config;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework;

public class ConfigurationDbContext : DbContext
{
    public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : base(options)
    {
    }

    public DbSet<AuthorizationDetailSchema> AuthorizationDetailSchemas => Set<AuthorizationDetailSchema>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ApiResource> ApiResources => Set<ApiResource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApiResourceConfiguration())
            .ApplyConfiguration(new AuthorizationDetailSchemaConfiguration())
            .ApplyConfiguration(new AuthorizationDetailSchemaActionConfiguration())
            .ApplyConfiguration(new ClientConfiguration())
            .ApplyConfiguration(new UserClaimConfiguration());
    }
}
