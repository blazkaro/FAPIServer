using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IAuthorizationCodeStore
{
    Task StoreAsync(AuthorizationCode authorizationCode, CancellationToken cancellationToken = default);
    Task<AuthorizationCode?> FindByCodeAndClientIdAsync(string code, string clientId, CancellationToken cancellationToken = default);
    Task RemoveAsync(string code, CancellationToken cancellationToken = default);
}
