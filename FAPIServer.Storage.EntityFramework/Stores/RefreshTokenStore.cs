using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class RefreshTokenStore : IRefreshTokenStore
{
    private readonly GrantsDbContext _dbContext;

    public RefreshTokenStore(GrantsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens.AsNoTrackingWithIdentityResolution()
            .Where(p => p.Token == token)
            .Include(p => p.Grant)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAllByGrantIdAsync(string grantId, CancellationToken cancellationToken = default)
    {
        var toRemove = await _dbContext.RefreshTokens.Where(p => p.Grant.GrantId == grantId).Select(p => p.Token).ToListAsync(cancellationToken);
        _dbContext.RefreshTokens.RemoveRange(toRemove.Select(p => new RefreshToken { Token = p }));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(string token, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Remove(new RefreshToken { Token = token });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        if (refreshToken.Grant is not null && _dbContext.Grants.Entry(refreshToken.Grant).State == EntityState.Detached)
        {
            _dbContext.Grants.Attach(refreshToken.Grant);
        }

        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(string token, Action<RefreshToken> update, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.RefreshTokens.FindAsync(new string[] { token }, cancellationToken);

        if (entity != null)
        {
            update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
