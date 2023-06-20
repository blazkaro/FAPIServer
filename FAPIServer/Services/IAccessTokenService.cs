using FAPIServer.Models;

namespace FAPIServer.Services;

public interface IAccessTokenService
{
    Task<string> GenerateAsync(AccessTokenPayload atPayload, CancellationToken cancellationToken = default);
}
