using FAPIServer.Models;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;

namespace FAPIServer.ResponseHandling.Default;

public class TokenIntrospectionResponseGenerator : ITokenIntrospectionResponseGenerator
{
    private readonly IApiResourceStore _apiResourceStore;

    public TokenIntrospectionResponseGenerator(IApiResourceStore apiResourceStore)
    {
        _apiResourceStore = apiResourceStore;
    }

    public async Task<TokenIntrospectionResponse> GenerateAsync(AccessTokenPayload atPayload, Client client, CancellationToken cancellationToken = default)
    {
        if (atPayload is null)
            throw new ArgumentNullException(nameof(atPayload));

        if (client is null)
            throw new ArgumentNullException(nameof(client));

        var authorizationDetails = atPayload.AuthorizationDetails;

        // When client id (access token owner) is not equal to client id that requested this introspection, then it can be resource server. Check it
        if (atPayload.ClientId != client.ClientId)
        {
            var apiResource = await _apiResourceStore.FindByClientIdAsync(client.ClientId, cancellationToken);

            // When the client which requested introspection is not authorized party and is not resource server
            if (apiResource == null)
                return new() { Active = false };

            // If no authorization details then is not active for resource server
            if (authorizationDetails is null || !authorizationDetails.Any())
                return new() { Active = false };

            // Whether the resource server is token audience
            var audiences = authorizationDetails.SelectMany(p => p.Locations);
            if (audiences.Contains(apiResource.Uri.ToString()))
                return new() { Active = false };

            // Override authorization_details to return only these that are handled by API resource
            authorizationDetails = authorizationDetails.Where(p => apiResource.HandledAuthorizationDetailTypes.Contains(p.Type));
        }

        return new()
        {
            Active = true,
            ClientId = atPayload.ClientId,
            TokenType = Constants.SupportedAccessTokenTypes.DPoP,
            NotBefore = atPayload.NotBefore,
            Expiration = atPayload.Expiration,
            Sub = atPayload.Subject,
            Cnf = atPayload.Cnf,
            AuthorizationDetails = authorizationDetails,
            TokenIdentifier = atPayload.Jti.ToString()
        };
    }

    public Task<TokenIntrospectionResponse> GenerateAsync(RefreshToken refreshToken, Client client, CancellationToken cancellationToken = default)
    {
        if (refreshToken is null)
            throw new ArgumentNullException(nameof(refreshToken));

        if (client is null)
            throw new ArgumentNullException(nameof(client));

        if (refreshToken.ClientId != client.ClientId)
            throw new InvalidOperationException("The refresh token is bounded to different client than provided");

        return Task.FromResult(new TokenIntrospectionResponse
        {
            Active = true,
            ClientId = refreshToken.ClientId,
            Expiration = refreshToken.ExpiresAt,
            Sub = refreshToken.Subject,
            AuthorizationDetails = refreshToken.AuthorizationDetails
        });
    }
}
