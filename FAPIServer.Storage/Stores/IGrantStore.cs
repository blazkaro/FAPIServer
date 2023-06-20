using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IGrantStore
{
    Task StoreAsync(Grant grant, CancellationToken cancellationToken = default);
    Task<Grant?> FindByGrantIdAndClientId(string grantId, string clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Grant>> FindAllBySubjectAndClientId(string subject, string clientId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string grantId, string clientId, CancellationToken cancellationToken = default);
    Task RemoveAsync(string grantId, CancellationToken cancellationToken = default);
    Task UpdateAsync(string grantId, Action<Grant> update, CancellationToken cancellationToken = default);
}
