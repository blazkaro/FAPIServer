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

    private async Task<PasetoBuilder> GetBasis(string issuer, string audience, string subject, Lifetime lifetime, CancellationToken cancellationToken)
    {
        var ed25519KeyPair = await _secretCredentialsStore.GetSigningCredentials(true, cancellationToken);
        var utcNow = DateTime.UtcNow;

        var idTokenBuilder = new PasetoBuilder()
            .WithJsonSerializer(new PasetoPayloadSerializer())
            .UseV4(Purpose.Public)
            .WithSecretKey(ed25519KeyPair.PrivateKey.Value.Concat(ed25519KeyPair.PublicKey.Value).ToArray())
            .Issuer(issuer)
            .Audience(audience)
            .Subject(subject)
            .NotBefore(utcNow)
            .Expiration(utcNow.AddSeconds(lifetime.Seconds));

        return idTokenBuilder;
    }

    public async Task<string> GenerateAsync(string issuer, AuthorizationCode authorizationCode, Lifetime lifetime, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(issuer)) throw new ArgumentException($"'{nameof(issuer)}' cannot be null or empty.", nameof(issuer));
        if (authorizationCode is null) throw new ArgumentNullException(nameof(authorizationCode));
        if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

        var idToken = (await GetBasis(issuer, authorizationCode.ClientId, authorizationCode.Subject, lifetime, cancellationToken))
            .AddClaim("nonce", authorizationCode.Nonce)
            .AddClaim("auth_time", authorizationCode.AuthTime)
            .Encode();

        return idToken;
    }

    public async Task<string> GenerateAsync(string issuer, CibaObject cibaObject, Lifetime lifetime, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(issuer)) throw new ArgumentException($"'{nameof(issuer)}' cannot be null or empty.", nameof(issuer));
        if (cibaObject is null) throw new ArgumentNullException(nameof(cibaObject));
        if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

        var idToken = (await GetBasis(issuer, cibaObject.ClientId, cibaObject.Subject, lifetime, cancellationToken))
            .Encode();

        return idToken;
    }
}
