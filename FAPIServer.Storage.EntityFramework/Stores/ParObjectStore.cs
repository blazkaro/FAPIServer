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
        return await _dbContext.ParObjects.AsNoTracking()
            .Where(p => p.Uri == uri && p.ClientId == clientId)
            .Include(p => p.Grant)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(string uri, CancellationToken cancellationToken = default)
    {
        _dbContext.ParObjects.Remove(new ParObject { Uri = uri });
        await _dbContext.SaveChangesAsync(cancellationToken);

    }

    public async Task StoreAsync(ParObject parObject, CancellationToken cancellationToken = default)
    {
        if (parObject.Grant is not null && _dbContext.Grants.Entry(parObject.Grant).State == EntityState.Detached)
        {
            _dbContext.Grants.Attach(parObject.Grant);
        }

        _dbContext.ParObjects.Add(parObject);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
