using FAPIServer.Storage.EntityFramework.Entities.RevocationsContext.Config;
using FAPIServer.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework;

public class RevocationsDbContext : DbContext
{
    public RevocationsDbContext(DbContextOptions<RevocationsDbContext> options) : base(options)
    {
    }

    public DbSet<RevokedAccessToken> RevokedAccessTokens => Set<RevokedAccessToken>();
    public DbSet<RevokedClientAssertion> RevokedClientAssertions => Set<RevokedClientAssertion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RevokedAccessTokenConfiguration())
            .ApplyConfiguration(new RevokedClientAssertionConfiguration());
    }
}
