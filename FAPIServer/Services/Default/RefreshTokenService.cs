using Base64Url;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using System.Security.Cryptography;

namespace FAPIServer.Services.Default;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenStore _refreshTokenStore;

    public RefreshTokenService(IRefreshTokenStore refreshTokenStore)
    {
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<string> GenerateAsync(AuthorizationCode authorizationCode, Lifetime lifetime, CancellationToken cancellationToken = default)
    {
        if (authorizationCode is null) throw new ArgumentNullException(nameof(authorizationCode));
        if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

        if (authorizationCode.Grant is null)
            throw new InvalidOperationException($"The authorization code used to generate refresh token must have non-null {nameof(authorizationCode.Grant)} property");

        var token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        var refreshToken = new RefreshToken
        {
            Token = token,
            ClientId = authorizationCode.ClientId,
            Subject = authorizationCode.Subject,
            AuthorizationDetails = authorizationCode.AuthorizationDetails,
            Claims = authorizationCode.Claims,
            Grant = authorizationCode.Grant,
            ExpiresAt = DateTime.UtcNow.AddSeconds(lifetime.Seconds)
        };

        await _refreshTokenStore.StoreAsync(refreshToken, cancellationToken);

        return refreshToken.Token;
    }

    public async Task<string> RotateAsync(RefreshToken refreshToken, Client client, CancellationToken cancellationToken = default)
    {
        if (refreshToken is null) throw new ArgumentNullException(nameof(refreshToken));
        if (client is null) throw new ArgumentNullException(nameof(client));

        var token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        await _refreshTokenStore.UpdateAsync(refreshToken.Token, update =>
        {
            update.Token = token;
            update.ExpiresAt = client.SlidingRefreshToken ? DateTime.UtcNow.AddMinutes(client.RefreshTokenLifetime.Seconds) : refreshToken.ExpiresAt;
        }, cancellationToken);

        return token;
    }
}
