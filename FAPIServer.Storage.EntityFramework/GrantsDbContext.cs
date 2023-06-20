using FAPIServer.Storage.EntityFramework.Entities.GrantsContext.Config;
using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework;

public class GrantsDbContext : DbContext
{
    public GrantsDbContext(DbContextOptions<GrantsDbContext> options) : base(options)
    {
    }

    public DbSet<Grant> Grants => Set<Grant>();
    public DbSet<ParObject> ParObjects => Set<ParObject>();
    public DbSet<AuthorizationCode> AuthorizationCodes => Set<AuthorizationCode>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GrantConfiguration())
            .ApplyConfiguration(new ParObjectConfiguration())
            .ApplyConfiguration(new AuthorizationCodeConfiguration())
            .ApplyConfiguration(new RefreshTokenConfiguration());
    }
}
