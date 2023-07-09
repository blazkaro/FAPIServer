using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IRefreshTokenStore
{
    Task StoreAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RemoveAsync(string token, CancellationToken cancellationToken = default);
    //Task RemoveAllByGrantIdAsync(string grantId, CancellationToken cancellationToken = default);
    //Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default, params Expression<Func<RefreshToken, object>>[] properties);
}
