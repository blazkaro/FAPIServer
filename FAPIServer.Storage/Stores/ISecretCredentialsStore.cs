using FAPIServer.Storage.Models;

namespace FAPIServer.Storage.Stores;

public interface ISecretCredentialsStore
{
    Task<Ed25519KeyPair> GetSigningCredentials(bool exposePrivateKey = false, CancellationToken cancellationToken = default);
}
