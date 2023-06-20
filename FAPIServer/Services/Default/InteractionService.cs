using Base64Url;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Models;
using System.Security.Cryptography;

namespace FAPIServer.Services.Default;

public class InteractionService : IInteractionService
{
    private readonly IAuthorizationRequestPersistenceService _authorizationRequestPersistenceService;
    private readonly IGrantStore _grantStore;

    public InteractionService(IAuthorizationRequestPersistenceService authorizationRequestPersistenceService,
        IGrantStore grantStore)
    {
        _authorizationRequestPersistenceService = authorizationRequestPersistenceService;
        _grantStore = grantStore;
    }

    public async Task DenyConsentAsync(ParObject parObject, CancellationToken cancellationToken = default)
    {
        if (parObject is null)
            throw new ArgumentNullException(nameof(parObject));

        await _authorizationRequestPersistenceService.UpdateAsync(parObject, update =>
        {
            update.WasConsentPageShown = true;
            update.AccessDenied = true;
        }, cancellationToken);
    }

    public async Task GrantConsentAsync(ParObject parObject, ValidatedUser user, IEnumerable<AuthorizationDetail>? grantedAuthorizationDetails, IEnumerable<string>? grantedClaims,
        CancellationToken cancellationToken = default)
    {
        if (parObject is null)
            throw new ArgumentNullException(nameof(parObject));

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        grantedAuthorizationDetails ??= Array.Empty<AuthorizationDetail>();
        grantedClaims ??= Array.Empty<string>();

        if (!grantedAuthorizationDetails.Any() && !grantedClaims.Any())
        {
            await DenyConsentAsync(parObject, cancellationToken);
            return;
        }

        switch (parObject.GrantManagementAction)
        {
            case Constants.SupportedGrantManagementActions.Create or null or "":
                if (!grantedClaims.Contains(Constants.BuiltInClaims.Subject))
                    throw new InvalidOperationException($"The '{Constants.BuiltInClaims.Subject}' claim must be granted");

                var grant = new Grant
                {
                    GrantId = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32)),
                    ClientId = parObject.ClientId,
                    Subject = user.Subject,
                    Claims = grantedClaims.ToList(),
                    AuthorizationDetails = grantedAuthorizationDetails.ToList(),
                    GrantedAt = DateTime.UtcNow
                };

                await _grantStore.StoreAsync(grant, cancellationToken);

                await _authorizationRequestPersistenceService.UpdateAsync(parObject, update =>
                {
                    update.WasConsentPageShown = true;
                    update.FreshGrant = grant;
                }, cancellationToken);

                break;

            case Constants.SupportedGrantManagementActions.Merge:
                await _authorizationRequestPersistenceService.UpdateAsync(parObject, update => update.WasConsentPageShown = true, cancellationToken);

                var currentGrant = parObject.RequestedGrant!;

                if (grantedAuthorizationDetails.Any() && currentGrant.AuthorizationDetails is null)
                    currentGrant.AuthorizationDetails = new List<AuthorizationDetail>();

                foreach (var authorizationDetail in grantedAuthorizationDetails)
                {
                    var toMerge = currentGrant.AuthorizationDetails.SingleOrDefault(p => p.Type == authorizationDetail.Type);

                    if (toMerge != null)
                        toMerge.Merge(authorizationDetail);
                    else
                        currentGrant.AuthorizationDetails.Add(authorizationDetail);
                }

                if (grantedClaims.Any() && currentGrant.Claims is null)
                    currentGrant.Claims = new HashSet<string>();

                if (grantedClaims.Any())
                {
                    currentGrant.Claims = (currentGrant.Claims ?? new HashSet<string>()).Concat(grantedClaims).ToHashSet();
                }

                await _grantStore.UpdateAsync(currentGrant.GrantId, update =>
                {
                    update.AuthorizationDetails = currentGrant.AuthorizationDetails;
                    update.Claims = currentGrant.Claims;
                }, cancellationToken);

                break;

            case Constants.SupportedGrantManagementActions.Replace:
                await _authorizationRequestPersistenceService.UpdateAsync(parObject, update => update.WasConsentPageShown = true, cancellationToken);

                await _grantStore.UpdateAsync(parObject.RequestedGrant!.GrantId, update =>
                {
                    update.AuthorizationDetails = grantedAuthorizationDetails.ToHashSet();
                    update.Claims = grantedClaims.ToHashSet();
                }, cancellationToken);

                break;

            default:
                throw new InvalidOperationException($"The '{nameof(parObject.GrantManagementAction)}' is not supported Grant Management Action");
        }
    }

    public async Task SignedIn(ParObject parObject, CancellationToken cancellationToken = default)
    {
        await _authorizationRequestPersistenceService.UpdateAsync(parObject, update =>
        {
            update.WasUserReauthenticated = true;
        }, cancellationToken);
    }
}
