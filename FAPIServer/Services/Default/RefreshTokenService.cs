using Base64Url;
using FAPIServer.Storage;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using System.Security.Cryptography;

namespace FAPIServer.Services.Default;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IChangesTracker<RefreshToken> _changesTracker;

    public RefreshTokenService(IRefreshTokenStore refreshTokenStore, IChangesTracker<RefreshToken> changesTracker)
    {
        _refreshTokenStore = refreshTokenStore;
        _changesTracker = changesTracker;
    }

    private async Task<RefreshToken> Generate(string clientId, string subject, IEnumerable<AuthorizationDetail> authorizationDetails, IEnumerable<string> claims,
        Grant grant, Lifetime lifetime, CancellationToken cancellationToken)
    {
        var token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        var refreshToken = new RefreshToken
        {
            Token = token,
            ClientId = clientId,
            Subject = subject,
            AuthorizationDetails = authorizationDetails,
            Claims = claims,
            Grant = grant,
            ExpiresAt = DateTime.UtcNow.AddSeconds(lifetime.Seconds)
        };

        await _refreshTokenStore.StoreAsync(refreshToken, cancellationToken);
        return refreshToken;
    }

    public async Task<RefreshToken> GenerateAsync(AuthorizationCode authorizationCode, Lifetime lifetime, CancellationToken cancellationToken = default)
    {
        if (authorizationCode is null) throw new ArgumentNullException(nameof(authorizationCode));
        if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

        if (authorizationCode.Grant is null)
            throw new InvalidOperationException($"The authorization code used to generate refresh token must have non-null {nameof(authorizationCode.Grant)} property");

        return await Generate(authorizationCode.ClientId, authorizationCode.Subject,
            authorizationCode.AuthorizationDetails, authorizationCode.Claims, authorizationCode.Grant, lifetime, cancellationToken);
    }

    public async Task<RefreshToken> GenerateAsync(CibaObject cibaObject, Lifetime lifetime, CancellationToken cancellationToken = default)
    {
        if (cibaObject is null) throw new ArgumentNullException(nameof(cibaObject));
        if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

        if (cibaObject.Grant is null)
            throw new InvalidOperationException($"The CIBA object used to generate refresh token must have non-null requested or fresh grant property");

        return await Generate(cibaObject.ClientId, cibaObject.Subject, cibaObject.AuthorizationDetails, cibaObject.Claims,
            cibaObject.Grant, lifetime, cancellationToken);
    }

    public async Task<RefreshToken> RotateAsync(RefreshToken refreshToken, Client client, CancellationToken cancellationToken = default)
    {
        if (refreshToken is null) throw new ArgumentNullException(nameof(refreshToken));
        if (client is null) throw new ArgumentNullException(nameof(client));

        _changesTracker.BeginTracking(refreshToken);

        refreshToken.Token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        refreshToken.ExpiresAt = client.SlidingRefreshToken ? DateTime.UtcNow.AddMinutes(client.RefreshTokenLifetime.Seconds) : refreshToken.ExpiresAt;

        await _changesTracker.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }
}
