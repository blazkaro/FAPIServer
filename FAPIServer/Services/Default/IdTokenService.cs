using FAPIServer.Serializers;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using Paseto;
using Paseto.Builder;

namespace FAPIServer.Services.Default;

public class IdTokenService : IIdTokenService
{
    private readonly ISecretCredentialsStore _secretCredentialsStore;

    public IdTokenService(ISecretCredentialsStore secretCredentialsStore)
    {
        _secretCredentialsStore = secretCredentialsStore;
    }

    public async Task<string> GenerateAsync(string issuer, AuthorizationCode authorizationCode, Lifetime lifetime, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(issuer)) throw new ArgumentException($"'{nameof(issuer)}' cannot be null or empty.", nameof(issuer));
        if (authorizationCode is null) throw new ArgumentNullException(nameof(authorizationCode));
        if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

        var ed25519KeyPair = await _secretCredentialsStore.GetSigningCredentials(true, cancellationToken);
        var utcNow = DateTime.UtcNow;

        var idToken = new PasetoBuilder()
            .WithJsonSerializer(new PasetoPayloadSerializer())
            .UseV4(Purpose.Public)
            .WithSecretKey(ed25519KeyPair.PrivateKey.Value.Concat(ed25519KeyPair.PublicKey.Value).ToArray())
            .Issuer(issuer)
            .Audience(authorizationCode.ClientId)
            .Subject(authorizationCode.Subject)
            .NotBefore(utcNow)
            .Expiration(utcNow.AddSeconds(lifetime.Seconds))
            .AddClaim("nonce", authorizationCode.Nonce)
            .AddClaim("auth_time", authorizationCode.AuthTime)
            .Encode();

        return idToken;
    }
}
