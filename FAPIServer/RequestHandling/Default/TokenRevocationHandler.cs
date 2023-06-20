using FAPIServer.Authentication;
using FAPIServer.Extensions;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation;
using Paseto;

namespace FAPIServer.RequestHandling.Default;

public class TokenRevocationHandler : ITokenRevocationHandler
{
    private readonly IClientAuthenticator _clientAuthenticator;
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IRevokedAccessTokenStore _revokedAccessTokenStore;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public TokenRevocationHandler(IClientAuthenticator clientAuthenticator,
        IAccessTokenValidator accessTokenValidator,
        IRevokedAccessTokenStore revokedAccessTokenStore,
        IRefreshTokenStore refreshTokenStore)
    {
        _clientAuthenticator = clientAuthenticator;
        _accessTokenValidator = accessTokenValidator;
        _revokedAccessTokenStore = revokedAccessTokenStore;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<TokenRevocationHandlerResult> HandleAsync(TokenRevocationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var authResult = await _clientAuthenticator.AuthenticateAsync(
            new ClientAuthenticationContext(context.AuthRequest, context.RequestedUri), cancellationToken);

        if (!authResult.IsAuthenticated)
            return new(authResult.Error, authResult.FailureMessage);

        if (context.Request.Token.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("token"));

        switch (context.Request.Token)
        {
            case var at when at.StartsWith("v4.public."): await RevokeAccessToken(context, authResult.Client, cancellationToken); break;
            default: await RevokeRefreshToken(context, authResult.Client, cancellationToken); break;
        }

        return new() { Success = true };
    }

    private async Task RevokeAccessToken(TokenRevocationContext context, Client client, CancellationToken cancellationToken)
    {
        var validationResult = await _accessTokenValidator.ValidateAsync(context.ValidTokenIssuer, context.Request.Token, cancellationToken);
        if (!validationResult.IsValid) return;

        var azp = (string)validationResult.Payload.SingleOrDefault(p => p.Key == "azp").Value;
        if (azp != client.ClientId) return;

        var jti = (string)validationResult.Payload.SingleOrDefault(p => p.Key == PasetoRegisteredClaimNames.TokenIdentifier).Value;
        var exp = (DateTime)validationResult.Payload.SingleOrDefault(p => p.Key == PasetoRegisteredClaimNames.ExpirationTime).Value;

        await _revokedAccessTokenStore.StoreAsync(new RevokedAccessToken { Jti = jti, TokenExpiresAt = exp }, cancellationToken);
    }

    private async Task RevokeRefreshToken(TokenRevocationContext context, Client client, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenStore.FindByTokenAsync(context.Request.Token, cancellationToken);
        if (refreshToken == null || refreshToken.ClientId != client.ClientId || refreshToken.HasExpired()) return;

        await _refreshTokenStore.RemoveAsync(context.Request.Token, cancellationToken);
    }
}
