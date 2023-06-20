using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;

namespace FAPIServer.InMemory;

public class InMemorySecretCredentialsStore : ISecretCredentialsStore
{
    private readonly Ed25519KeyPair _ed25519KeyPair;

    public InMemorySecretCredentialsStore(Ed25519KeyPair ed25519KeyPair)
    {
        _ed25519KeyPair = ed25519KeyPair;
    }

    public Task<Ed25519KeyPair> GetSigningCredentials(bool exposePrivateKey = false, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(exposePrivateKey ? _ed25519KeyPair : new Ed25519KeyPair(_ed25519KeyPair.PublicKey));
    }
}
