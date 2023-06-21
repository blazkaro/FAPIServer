using FAPIServer.Models;
using FAPIServer.Serializers;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.Extensions.Options;
using Paseto;
using Paseto.Builder;

namespace FAPIServer.Authentication.Default;

public class ClientAuthenticator : IClientAuthenticator
{
    private readonly IClientStore _clientStore;
    private readonly FapiOptions _options;
    private readonly IRevokedClientAssertionStore _revokedClientAssertionStore;

    public ClientAuthenticator(IClientStore clientStore,
        IOptionsMonitor<FapiOptions> options,
        IRevokedClientAssertionStore revokedClientAssertionStore)
    {
        _clientStore = clientStore;
        _options = options.CurrentValue;
        _revokedClientAssertionStore = revokedClientAssertionStore;
    }

    public async Task<ClientAuthenticationResult> AuthenticateAsync(ClientAuthenticationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (string.IsNullOrEmpty(context.AuthRequest.ClientId))
            return Failed(Error.InvalidRequest, ErrorDescriptions.MissingParameter("client_id"));

        var client = await _clientStore.FindEnabledByClientIdAsync(context.AuthRequest.ClientId, cancellationToken);
        if (client == null) return Failed(Error.InvalidClient, "The client not found");

        return await PrivateKeyPasetoAuthMethod(context, client, cancellationToken);
    }

    private async Task<ClientAuthenticationResult> PrivateKeyPasetoAuthMethod(ClientAuthenticationContext context, Client client, CancellationToken cancellationToken)
    {
        if (context.AuthRequest.ClientAssertionType != Constants.SupportedClientAssertionTypes.PasetoBearer
            || string.IsNullOrEmpty(context.AuthRequest.ClientAssertion))
            return Failed(Error.InvalidClient, $"The '{Constants.SupportedClientAuthenticationMethods.PrivateKeyPaseto}' authentication method is required");

        var assertionValidationResult = new PasetoBuilder()
            .WithJsonSerializer(new PasetoPayloadSerializer())
            .UseV4(Purpose.Public)
            .WithPublicKey(client.Ed25519PublicKey.Value)
            .Decode(context.AuthRequest.ClientAssertion, new PasetoTokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateSubject = true,
                ValidAudience = context.ValidAudience.ToString(),
                ValidIssuer = client.ClientId,
                ValidSubject = client.ClientId
            });

        if (!assertionValidationResult.IsValid || !assertionValidationResult.Paseto.Payload.HasNotBefore() || !assertionValidationResult.Paseto.Payload.HasTokenIdentifier())
            return Failed(Error.InvalidClient, "Invalid client assertion");

        var assertionPayload = ClientAssertionPayload.FromJson(assertionValidationResult.Paseto.RawPayload)!;
        if (_options.UseClientAssertionRevocation)
        {
            var isRevoked = await _revokedClientAssertionStore.IsRevokedAsync(assertionPayload.Jti.ToString(), client.ClientId, cancellationToken);
            if (isRevoked) return Failed(Error.InvalidClient, "The client assertion is revoked");

            await _revokedClientAssertionStore.StoreAsync(new RevokedClientAssertion
            {
                Jti = assertionPayload.Jti.ToString(),
                ClientId = client.ClientId,
                AssertionExpiresAt = assertionPayload.Expiration
            }, cancellationToken);
        }

        return new(client, assertionPayload);
    }

    private static ClientAuthenticationResult Failed(Error error, string? failureMessage = null)
        => new(error, failureMessage);
}
