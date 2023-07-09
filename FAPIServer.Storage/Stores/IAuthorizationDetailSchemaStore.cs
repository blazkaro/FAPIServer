using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface IAuthorizationDetailSchemaStore : IDiscoveryStore
{
    Task<IEnumerable<AuthorizationDetailSchema>> FindEnabledByTypesAsync(IEnumerable<string> types, CancellationToken cancellationToken = default);
}
