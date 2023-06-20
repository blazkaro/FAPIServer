using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IApiResourceStore
{
    Task<IEnumerable<ApiResource>> FindByUrisAsync(IEnumerable<Uri> uris, CancellationToken cancellationToken = default);
    Task<ApiResource?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
}
