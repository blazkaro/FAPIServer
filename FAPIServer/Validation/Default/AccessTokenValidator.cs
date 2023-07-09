using FAPIServer.Extensions;
using FAPIServer.Models;
using FAPIServer.Serializers;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation.Results;
using Paseto;
using Paseto.Builder;

namespace FAPIServer.Validation.Default;

public class AccessTokenValidator : IAccessTokenValidator
{
    private readonly ISecretCredentialsStore _secretCredentialsStore;
    private readonly IRevokedAccessTokenStore _revokedAccessTokenStore;

    public AccessTokenValidator(ISecretCredentialsStore secretCredentialsStore,
        IRevokedAccessTokenStore revokedAccessTokenStore)
    {
        _secretCredentialsStore = secretCredentialsStore;
        _revokedAccessTokenStore = revokedAccessTokenStore;
    }

    public async Task<TokenValidationResult<AccessTokenPayload>> ValidateAsync(string validIssuer, string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(validIssuer))
            throw new ArgumentException($"'{nameof(validIssuer)}' cannot be null or empty.", nameof(validIssuer));

        if (string.IsNullOrEmpty(accessToken))
            throw new ArgumentException($"'{nameof(accessToken)}' cannot be null or empty.", nameof(accessToken));

        var publicKey = (await _secretCredentialsStore.GetSigningCredentials(false, cancellationToken)).PublicKey;
        var validationResult = new PasetoBuilder()
            .WithJsonSerializer(new PasetoPayloadSerializer())
            .UseV4(Purpose.Public)
            .WithPublicKey(publicKey.Value)
            .Decode(accessToken, new PasetoTokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateSubject = false,
                ValidIssuer = validIssuer
            });

        if (!validationResult.IsValid)
            return new(validationResult.Exception.Message);

        var payload = AccessTokenPayload.FromJson(validationResult.Paseto.RawPayload)!;
        if (payload.NotBefore == default)
            return TokenValidationResult<AccessTokenPayload>.MissingClaim(PasetoRegisteredClaimNames.NotBefore);

        if (payload.Jti == Guid.Empty)
            return TokenValidationResult<AccessTokenPayload>.MissingClaim(PasetoRegisteredClaimNames.TokenIdentifier);

        if (payload.ClientId.IsNullOrEmpty())
            return TokenValidationResult<AccessTokenPayload>.MissingClaim("client_id");

        if (payload.Cnf is null)
            return TokenValidationResult<AccessTokenPayload>.MissingClaim("cnf");

        if (payload.Cnf.Pkh.IsNullOrEmpty())
            return new("The 'pkh' property must be present in 'cnf' object");

        if (await _revokedAccessTokenStore.IsRevokedAsync(payload.Jti.ToString(), cancellationToken))
            return new("The access token is revoked");

        return new(payload);
    }
}
