using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class ParObjectStore : IParObjectStore
{
    private readonly GrantsDbContext _dbContext;

    public ParObjectStore(GrantsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ParObject?> FindByUriAndClientIdAsync(string uri, string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ParObjects.AsNoTrackingWithIdentityResolution()
            .Where(p => p.Uri == uri && p.ClientId == clientId)
            .Include(p => p.RequestedGrant)
            .Include(p => p.FreshGrant)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(string uri, CancellationToken cancellationToken = default)
    {
        _dbContext.ParObjects.Remove(new ParObject { Uri = uri });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreAsync(ParObject parObject, CancellationToken cancellationToken = default)
    {
        if (parObject.RequestedGrant is not null && _dbContext.Grants.Entry(parObject.RequestedGrant).State == EntityState.Detached)
        {
            _dbContext.Grants.Attach(parObject.RequestedGrant);
        }

        if (parObject.FreshGrant is not null && _dbContext.Grants.Entry(parObject.FreshGrant).State == EntityState.Detached)
        {
            _dbContext.Grants.Attach(parObject.FreshGrant);
        }

        await _dbContext.ParObjects.AddAsync(parObject, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(string uri, Action<ParObject> update, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ParObjects.FindAsync(new string[] { uri }, cancellationToken);
        if (entity != null)
        {
            update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
