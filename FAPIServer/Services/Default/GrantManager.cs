using Base64Url;
using FAPIServer.Storage;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using System.Security.Cryptography;

namespace FAPIServer.Services.Default;

public class GrantManager : IGrantManager
{
    private readonly IGrantStore _grantStore;
    private readonly IChangesTracker<Grant> _changesTracker;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public GrantManager(IGrantStore grantStore,
        IChangesTracker<Grant> changesTracker,
        IRefreshTokenStore refreshTokenStore)
    {
        _grantStore = grantStore;
        _changesTracker = changesTracker;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<Grant> CreateAsync(string clientId, string subject, IEnumerable<AuthorizationDetail> authorizationDetails, IEnumerable<string> claims,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));
        if (string.IsNullOrEmpty(subject)) throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty.", nameof(subject));
        if (authorizationDetails is null) throw new ArgumentNullException(nameof(authorizationDetails));
        if (claims is null) throw new ArgumentNullException(nameof(claims));

        var grant = new Grant
        {
            GrantId = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32)),
            ClientId = clientId,
            Subject = subject,
            Claims = claims.ToHashSet(),
            AuthorizationDetails = authorizationDetails?.ToHashSet() ?? new(),
            GrantedAt = DateTime.UtcNow
        };

        await _grantStore.StoreAsync(grant, cancellationToken);
        return grant;
    }

    public async Task MergeAsync(Grant grant, IEnumerable<AuthorizationDetail> authorizationDetails, IEnumerable<string> claims,
        CancellationToken cancellationToken = default)
    {
        if (grant is null) throw new ArgumentNullException(nameof(grant));
        if (authorizationDetails is null) throw new ArgumentNullException(nameof(authorizationDetails));
        if (claims is null) throw new ArgumentNullException(nameof(claims));

        _changesTracker.BeginTracking(grant);
        foreach (var authorizationDetail in authorizationDetails)
        {
            var toMerge = grant.AuthorizationDetails.SingleOrDefault(p => p.Type == authorizationDetail.Type);

            if (toMerge is not null) toMerge.Merge(authorizationDetail);
            else grant.AuthorizationDetails.Add(authorizationDetail);
        }

        grant.Claims = grant.Claims.Concat(claims).ToHashSet();

        await _changesTracker.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(Grant grant, IEnumerable<AuthorizationDetail> authorizationDetails, IEnumerable<string> claims,
        CancellationToken cancellationToken = default)
    {
        if (grant is null) throw new ArgumentNullException(nameof(grant));
        if (authorizationDetails is null) throw new ArgumentNullException(nameof(authorizationDetails));
        if (claims is null) throw new ArgumentNullException(nameof(claims));

        _changesTracker.BeginTracking(grant);

        grant.AuthorizationDetails = authorizationDetails.ToList();
        grant.Claims = claims.ToHashSet();

        await _changesTracker.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAsync(string grantId, CancellationToken cancellationToken = default)
    {
        //await _refreshTokenStore.RemoveAllByGrantIdAsync(grantId, cancellationToken);
        await _grantStore.RemoveAsync(grantId, cancellationToken);
    }
}
