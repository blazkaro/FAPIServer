using FAPIServer.Extensions;
using FAPIServer.Models;
using FAPIServer.Serializers;
using FAPIServer.Storage.Stores;
using Paseto;
using Paseto.Builder;

namespace FAPIServer.Services.Default;

public class AccessTokenService : IAccessTokenService
{
    private readonly ISecretCredentialsStore _secretCredentialsStore;

    public AccessTokenService(ISecretCredentialsStore secretCredentialsStore)
    {
        _secretCredentialsStore = secretCredentialsStore;
    }

    public async Task<string> GenerateAsync(AccessTokenPayload atPayload, CancellationToken cancellationToken = default)
    {
        var ed25519KeyPair = await _secretCredentialsStore.GetSigningCredentials(true, cancellationToken);

        var builder = new PasetoBuilder()
            .WithJsonSerializer(new PasetoPayloadSerializer())
            .UseV4(Purpose.Public)
            .WithSecretKey(ed25519KeyPair.PrivateKey.Value.Concat(ed25519KeyPair.PublicKey.Value).ToArray())
            .Issuer(atPayload.Issuer)
            .Subject(atPayload.Subject)
            .NotBefore(atPayload.NotBefore)
            .Expiration(atPayload.Expiration)
            .TokenIdentifier(atPayload.Jti.ToString())
            .AddClaim("client_id", atPayload.ClientId)
            .AddClaim("cnf", atPayload.Cnf);

        if (atPayload.AuthorizationDetails is not null && atPayload.AuthorizationDetails.Any())
            builder.AddClaim("authorization_details", atPayload.AuthorizationDetails);

        if (!atPayload.Claims.IsNullOrEmpty())
            builder.AddClaim("claims", atPayload.Claims);

        return builder.Encode();
    }
}
