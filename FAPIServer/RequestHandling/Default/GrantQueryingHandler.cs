using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Stores;

namespace FAPIServer.RequestHandling.Default;

public class GrantQueryingHandler : IGrantQueryingHandler
{
    private readonly IGrantStore _grantStore;

    public GrantQueryingHandler(IGrantStore grantStore)
    {
        _grantStore = grantStore;
    }

    public async Task<GrantQueryingHandlerResult> HandleAsync(GrantManagementContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var grant = await _grantStore.FindByGrantIdAndClientId(context.GrantId, context.AccessToken.ClientId, cancellationToken);
        if (grant == null) return new() { Success = false };

        return new(new GrantQueryingResponse
        {
            AuthorizationDetails = grant.AuthorizationDetails,
            Claims = grant.Claims
        });
    }
}
