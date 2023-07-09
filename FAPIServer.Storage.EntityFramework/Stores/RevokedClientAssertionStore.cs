using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class RevokedClientAssertionStore : IRevokedClientAssertionStore
{
    private readonly RevocationsDbContext _dbContext;

    public RevokedClientAssertionStore(RevocationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsRevokedAsync(string jti, string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RevokedClientAssertions.AnyAsync(p => p.Jti == jti && p.ClientId == clientId, cancellationToken);
    }

    public async Task StoreAsync(RevokedClientAssertion revokedClientAssertion, CancellationToken cancellationToken = default)
    {
        _dbContext.RevokedClientAssertions.Add(revokedClientAssertion);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
