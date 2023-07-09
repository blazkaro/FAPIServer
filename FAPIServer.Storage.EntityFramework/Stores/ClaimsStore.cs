using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class ClaimsStore : IClaimsStore
{
    private readonly ConfigurationDbContext _dbContext;

    public ClaimsStore(ConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserClaim>> FindEnabledByTypesAsync(IEnumerable<string> types, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserClaims.AsNoTracking()
            .Where(p => p.Enabled && types.Contains(p.Type))
            .Select(p => new UserClaim
            {
                Type = p.Type,
                Enabled = p.Enabled,
                Description = p.Description,
                ShowInDiscoveryDocument = p.ShowInDiscoveryDocument
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> FindDiscoverableAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserClaims.AsNoTracking()
            .Where(p => p.Enabled && p.ShowInDiscoveryDocument)
            .Select(p => p.Type)
            .ToListAsync(cancellationToken);
    }
}
