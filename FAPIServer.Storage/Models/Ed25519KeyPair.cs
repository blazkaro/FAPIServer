using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.Models;

public class Ed25519KeyPair
{
    public Ed25519KeyPair(Ed25519Key privateKey, Ed25519Key publicKey)
    {
        PrivateKey = privateKey;
        PublicKey = publicKey;
    }

    public Ed25519KeyPair(Ed25519Key publicKey)
    {
        PublicKey = publicKey;
    }

    public Ed25519Key PrivateKey { get; init; }
    public Ed25519Key PublicKey { get; init; }
}
