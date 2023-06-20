using FAPIServer.Extensions;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using System.Text.Json;

namespace FAPIServer.ResponseHandling.Default;

public class TokenIntrospectionResponseGenerator : ITokenIntrospectionResponseGenerator
{
    private readonly IApiResourceStore _apiResourceStore;

    public TokenIntrospectionResponseGenerator(IApiResourceStore apiResourceStore)
    {
        _apiResourceStore = apiResourceStore;
    }

    public async Task<TokenIntrospectionResponse> GenerateAsync(IEnumerable<KeyValuePair<string, object>> atPayload, Client client, CancellationToken cancellationToken = default)
    {
        if (atPayload is null)
            throw new ArgumentNullException(nameof(atPayload));

        if (client is null)
            throw new ArgumentNullException(nameof(client));

        var azp = (string)atPayload.SingleOrDefault(p => p.Key == "azp").Value;
        var authorizationDetails = AuthorizationDetailExtensions.ReadFromJson((string?)atPayload.SingleOrDefault(p => p.Key == "authorization_details").Value,
            out _);

        // When authorized party (access token owner) is not equal to client id that requested this introspection, then it can be resource server. Check it
        if (azp != client.ClientId)
        {
            var apiResource = await _apiResourceStore.FindByClientIdAsync(client.ClientId, cancellationToken);

            // When the client which requested introspection is not authorized party and is not resource server
            if (apiResource == null)
                return new() { Active = false };

            // Whether the resource server is token audience
            var audiences = authorizationDetails.SelectMany(p => p.Locations);
            if (audiences.Contains(apiResource.Uri.ToString()))
                return new() { Active = false };

            // Override authorization_details to return only these that are handled by API resource
            authorizationDetails = authorizationDetails.Where(p => apiResource.HandledAuthorizationDetailTypes.Contains(p.Type));
        }

        // If no authorization details then is not active
        if (!authorizationDetails.Any())
            return new() { Active = false };

        return new()
        {
            Active = true,
            ClientId = azp,
            TokenType = Constants.SupportedAccessTokenTypes.DPoP,
            NotBefore = (DateTime)atPayload.SingleOrDefault(p => p.Key == "nbf").Value,
            Expiration = (DateTime)atPayload.SingleOrDefault(p => p.Key == "exp").Value,
            Sub = (string)atPayload.SingleOrDefault(p => p.Key == "sub").Value,
            Cnf = JsonSerializer.Deserialize<object>((string)atPayload.SingleOrDefault(p => p.Key == "cnf").Value),
            AuthorizationDetails = authorizationDetails,
            TokenIdentifier = (string)atPayload.SingleOrDefault(p => p.Key == "jti").Value
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
