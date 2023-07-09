using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IParObjectStore
{
    Task StoreAsync(ParObject parObject, CancellationToken cancellationToken = default);
    Task<ParObject?> FindByUriAndClientIdAsync(string uri, string clientId, CancellationToken cancellationToken = default);
    Task RemoveAsync(string uri, CancellationToken cancellationToken = default);
    //Task UpdateAsync(ParObject parObject, CancellationToken cancellationToken = default, params Expression<Func<ParObject, object>>[] properties);
}
