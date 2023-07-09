using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class AuthorizationCodeStore : IAuthorizationCodeStore
{
    private readonly GrantsDbContext _dbContext;

    public AuthorizationCodeStore(GrantsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuthorizationCode?> FindByCodeAndClientIdAsync(string code, string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AuthorizationCodes.AsNoTracking()
            .Where(p => p.Code == code && p.ClientId == clientId)
            .Include(p => p.Grant)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(string code, CancellationToken cancellationToken = default)
    {
        _dbContext.AuthorizationCodes.Remove(new AuthorizationCode { Code = code });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreAsync(AuthorizationCode authorizationCode, CancellationToken cancellationToken = default)
    {
        if (authorizationCode.Grant is not null && _dbContext.Grants.Entry(authorizationCode.Grant).State == EntityState.Detached)
            _dbContext.Grants.Attach(authorizationCode.Grant);

        await _dbContext.AuthorizationCodes.AddAsync(authorizationCode, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
