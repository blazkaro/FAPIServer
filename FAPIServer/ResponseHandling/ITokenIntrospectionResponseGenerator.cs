using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Models;

namespace FAPIServer.ResponseHandling;

public interface ITokenIntrospectionResponseGenerator
{
    Task<TokenIntrospectionResponse> GenerateAsync(IEnumerable<KeyValuePair<string, object>> atPayload, Client client, CancellationToken cancellationToken = default);
    Task<TokenIntrospectionResponse> GenerateAsync(RefreshToken refreshToken, Client client, CancellationToken cancellationToken = default);
}
