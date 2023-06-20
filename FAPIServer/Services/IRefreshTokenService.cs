using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Services;

public interface IRefreshTokenService
{
    Task<string> GenerateAsync(AuthorizationCode authorizationCode, Lifetime lifetime, CancellationToken cancellationToken = default);
    Task<string> RotateAsync(RefreshToken refreshToken, Client client, CancellationToken cancellationToken = default);
}
