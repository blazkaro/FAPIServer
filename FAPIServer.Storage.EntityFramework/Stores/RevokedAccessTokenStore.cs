using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class RevokedAccessTokenStore : IRevokedAccessTokenStore
{
    private readonly RevocationsDbContext _dbContext;

    public RevokedAccessTokenStore(RevocationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RevokedAccessTokens.AnyAsync(p => p.Jti == jti, cancellationToken);
    }

    public async Task StoreAsync(RevokedAccessToken revokedAccessToken, CancellationToken cancellationToken = default)
    {
        _dbContext.RevokedAccessTokens.Add(revokedAccessToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
