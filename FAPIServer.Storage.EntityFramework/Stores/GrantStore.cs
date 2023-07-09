using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class GrantStore : IGrantStore
{
    private readonly GrantsDbContext _dbContext;

    public GrantStore(GrantsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(string grantId, string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Grants.AsNoTracking().AnyAsync(p => p.GrantId == grantId && p.ClientId == clientId, cancellationToken);
    }

    public async Task<IEnumerable<Grant>> FindAllBySubjectAndClientId(string subject, string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Grants.AsNoTracking()
            .Where(p => p.Subject == subject && p.ClientId == clientId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Grant?> FindByGrantIdAndClientId(string grantId, string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Grants.AsNoTracking()
            .Where(p => p.GrantId == grantId && p.ClientId == clientId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(string grantId, CancellationToken cancellationToken = default)
    {
        _dbContext.Grants.Remove(new Grant { GrantId = grantId });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreAsync(Grant grant, CancellationToken cancellationToken = default)
    {
        _dbContext.Grants.Add(grant);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
