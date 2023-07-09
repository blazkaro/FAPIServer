using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Services;
using FAPIServer.Storage.Stores;

namespace FAPIServer.RequestHandling.Default;

public class GrantRevocationHandler : IGrantRevocationHandler
{
    private readonly IGrantStore _grantStore;
    private readonly IGrantManager _grantManager;

    public GrantRevocationHandler(IGrantStore grantStore, IGrantManager grantManager)
    {
        _grantStore = grantStore;
        _grantManager = grantManager;
    }

    public async Task<bool> HandleAsync(GrantManagementContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (!await _grantStore.ExistsAsync(context.GrantId, context.AccessToken.ClientId, cancellationToken))
            return false;

        await _grantManager.RevokeAsync(context.GrantId, cancellationToken);

        return true;
    }
}
