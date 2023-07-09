using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateAsync(AuthorizationCode authorizationCode, Lifetime lifetime, CancellationToken cancellationToken = default);
    Task<RefreshToken> GenerateAsync(CibaObject cibaObject, Lifetime lifetime, CancellationToken cancellationToken = default);
    Task<RefreshToken> RotateAsync(RefreshToken refreshToken, Client client, CancellationToken cancellationToken = default);
}
