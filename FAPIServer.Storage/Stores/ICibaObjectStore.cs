using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface ICibaObjectStore
{
    Task StoreAsync(CibaObject cibaObject, CancellationToken cancellationToken = default);
    Task<CibaObject?> FindByIdAndClientIdAsync(string id, string clientId, CancellationToken cancellationToken = default);
    //Task UpdateAsync(CibaObject cibaObject, CancellationToken cancellationToken = default, params Expression<Func<CibaObject, object>>[] properties);
    Task RemoveAsync(string id, CancellationToken cancellationToken = default);
}
