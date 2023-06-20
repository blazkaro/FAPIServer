using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Storage.Stores;

namespace FAPIServer.RequestHandling.Default;

public class GrantRevocationHandler : IGrantRevocationHandler
{
    private readonly IGrantStore _grantStore;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public GrantRevocationHandler(IGrantStore grantStore,
        IRefreshTokenStore refreshTokenStore)
    {
        _grantStore = grantStore;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<bool> HandleAsync(GrantManagementContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (!await _grantStore.ExistsAsync(context.GrantId, context.AccessToken.ClientId, cancellationToken))
            return false;

        await _refreshTokenStore.RemoveAllByGrantIdAsync(context.GrantId, cancellationToken);
        await _grantStore.RemoveAsync(context.GrantId, cancellationToken);

        return true;
    }
}
