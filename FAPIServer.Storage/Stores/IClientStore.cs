using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IClientStore
{
    Task<Client?> FindEnabledByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
}
