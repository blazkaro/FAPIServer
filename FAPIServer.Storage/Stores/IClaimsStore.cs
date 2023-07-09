using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IClaimsStore : IDiscoveryStore
{
    Task<IEnumerable<UserClaim>> FindEnabledByTypesAsync(IEnumerable<string> types, CancellationToken cancellationToken = default);
}
