using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class CibaObjectStore : ICibaObjectStore
{
    private readonly GrantsDbContext _dbContext;

    public CibaObjectStore(GrantsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CibaObject?> FindByIdAndClientIdAsync(string id, string clientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));

        return await _dbContext.CibaObjects.AsNoTracking()
            .Where(p => p.Id == id && p.ClientId == clientId)
            .Include(p => p.Grant)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        _dbContext.CibaObjects.Remove(new CibaObject { Id = id });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreAsync(CibaObject cibaObject, CancellationToken cancellationToken = default)
    {
        _dbContext.CibaObjects.Add(cibaObject);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
