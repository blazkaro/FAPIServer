using FAPIServer.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation.Results;
using Paseto;
using Paseto.Builder;

namespace FAPIServer.Validation.Default;

public class IdTokenValidator : IIdTokenValidator
{
    private readonly ISecretCredentialsStore _secretCredentialsStore;

    public IdTokenValidator(ISecretCredentialsStore secretCredentialsStore)
    {
        _secretCredentialsStore = secretCredentialsStore;
    }

    public async Task<TokenValidationResult<IdTokenPayload>> ValidateAsync(string idToken, string validIssuer, string validAudience,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(idToken))
            throw new ArgumentException($"'{nameof(idToken)}' cannot be null or empty.", nameof(idToken));

        if (string.IsNullOrEmpty(validIssuer))
            throw new ArgumentException($"'{nameof(validIssuer)}' cannot be null or empty.", nameof(validIssuer));

        if (string.IsNullOrEmpty(validAudience))
            throw new ArgumentException($"'{nameof(validAudience)}' cannot be null or empty.", nameof(validAudience));

        var publicKey = (await _secretCredentialsStore.GetSigningCredentials(false, cancellationToken)).PublicKey;
        var validationResult = new PasetoBuilder()
            .UseV4(Purpose.Public)
            .WithPublicKey(publicKey.Value)
            .Decode(idToken, new PasetoTokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateSubject = false,
                ValidIssuer = validIssuer,
                ValidAudience = validAudience
            });

        if (!validationResult.IsValid)
            return new(validationResult.Exception.Message);

        var payload = IdTokenPayload.FromJson(validationResult.Paseto.RawPayload)!;
        return new(payload);
    }
}
