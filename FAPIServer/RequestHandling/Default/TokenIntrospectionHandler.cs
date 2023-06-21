using FAPIServer.Authentication;
using FAPIServer.Extensions;
using FAPIServer.Models;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;
using FAPIServer.ResponseHandling;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation;

namespace FAPIServer.RequestHandling.Default;

public class TokenIntrospectionHandler : ITokenIntrospectionHandler
{
    private readonly IClientAuthenticator _clientAuthenticator;
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly ITokenIntrospectionResponseGenerator _responseGenerator;

    public TokenIntrospectionHandler(IClientAuthenticator clientAuthenticator,
        IAccessTokenValidator accessTokenValidator,
        IRefreshTokenStore refreshTokenStore,
        ITokenIntrospectionResponseGenerator responseGenerator)
    {
        _clientAuthenticator = clientAuthenticator;
        _accessTokenValidator = accessTokenValidator;
        _refreshTokenStore = refreshTokenStore;
        _responseGenerator = responseGenerator;
    }

    public async Task<TokenIntrospectionHandlerResult> HandleAsync(TokenIntrospectionContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var authResult = await _clientAuthenticator.AuthenticateAsync(
            new ClientAuthenticationContext(context.AuthRequest, context.RequestedUri), cancellationToken);

        if (!authResult.IsAuthenticated)
            return new(authResult.Error, authResult.FailureMessage);

        if (context.Request.Token.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("token"));

        return context.Request.Token switch
        {
            var at when at.StartsWith("v4.public.") => await IntrospectAccessToken(context, authResult.Client, cancellationToken),
            _ => await IntrospectRefreshToken(context, authResult.Client, cancellationToken)
        };
    }

    private async Task<TokenIntrospectionHandlerResult> IntrospectAccessToken(TokenIntrospectionContext context, Client client, CancellationToken cancellationToken)
    {
        var validationResult = await _accessTokenValidator.ValidateAsync(context.ValidTokenIssuer, context.Request.Token, cancellationToken);
        if (!validationResult.IsValid)
            return new(new TokenIntrospectionResponse { Active = false });

        return new(await _responseGenerator.GenerateAsync(new AccessTokenPayload(validationResult.Payload), client, cancellationToken));
    }

    private async Task<TokenIntrospectionHandlerResult> IntrospectRefreshToken(TokenIntrospectionContext context, Client client, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenStore.FindByTokenAsync(context.Request.Token, cancellationToken);
        // Refresh token must be bounded to the same client that requested introspection.
        // After all, resource server won't (or even shouldn't) introspect refresh token
        if (refreshToken == default || refreshToken.ClientId != client.ClientId)
            return new(new TokenIntrospectionResponse { Active = false });

        return new(await _responseGenerator.GenerateAsync(refreshToken, client, cancellationToken));
    }
}
