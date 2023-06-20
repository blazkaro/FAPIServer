using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IRevokedAccessTokenStore
{
    Task StoreAsync(RevokedAccessToken revokedAccessToken, CancellationToken cancellationToken = default);
    Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default);
}