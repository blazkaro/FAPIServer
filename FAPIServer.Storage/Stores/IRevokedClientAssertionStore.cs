using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IRevokedClientAssertionStore
{
    Task StoreAsync(RevokedClientAssertion revokedClientAssertion, CancellationToken cancellationToken = default);
    Task<bool> IsRevokedAsync(string jti, string clientId, CancellationToken cancellationToken = default);
}
