namespace FAPIServer.Storage.Stores;

public interface IDiscoveryStore
{
    Task<IEnumerable<string>> FindDiscoverableAsync(CancellationToken cancellationToken = default);
}
