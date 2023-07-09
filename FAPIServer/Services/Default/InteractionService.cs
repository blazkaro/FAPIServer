using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Models;

namespace FAPIServer.Services.Default;

public class InteractionService : IInteractionService
{
    private readonly IAuthorizationRequestPersistenceService _authorizationRequestPersistenceService;
    private readonly IGrantManager _grantManager;

    public InteractionService(IAuthorizationRequestPersistenceService authorizationRequestPersistenceService,
        IGrantManager grantManager)
    {
        _authorizationRequestPersistenceService = authorizationRequestPersistenceService;
        _grantManager = grantManager;
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

    public async Task GrantConsentAsync(ParObject parObject, ValidatedUser user, IEnumerable<AuthorizationDetail>? grantedAuthorizationDetails,
        IEnumerable<string>? grantedClaims, CancellationToken cancellationToken = default)
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
            case Constants.GrantManagementActions.Create or null or "":
                if (!grantedClaims.Contains(Constants.BuiltInClaims.Subject))
                    throw new InvalidOperationException($"The '{Constants.BuiltInClaims.Subject}' claim must be granted");

                var grant = await _grantManager.CreateAsync(parObject.ClientId, user.Subject, grantedAuthorizationDetails, grantedClaims,
                    cancellationToken);

                await _authorizationRequestPersistenceService.UpdateAsync(parObject, update =>
                {
                    update.WasConsentPageShown = true;
                    update.Grant = grant;
                }, cancellationToken);

                break;

            case Constants.GrantManagementActions.Merge:
                await _authorizationRequestPersistenceService.UpdateAsync(parObject, update => update.WasConsentPageShown = true, cancellationToken);
                await _grantManager.MergeAsync(parObject.Grant!, grantedAuthorizationDetails, grantedClaims, cancellationToken);

                break;

            case Constants.GrantManagementActions.Replace:
                await _authorizationRequestPersistenceService.UpdateAsync(parObject, update => update.WasConsentPageShown = true, cancellationToken);
                await _grantManager.ReplaceAsync(parObject.Grant!, grantedAuthorizationDetails, grantedClaims, cancellationToken);

                break;

            default:
                throw new InvalidOperationException($"The '{nameof(parObject.GrantManagementAction)}' is not supported Grant Management Action");
        }
    }

    public async Task GrantConsentAsync(ParObject parObject, ValidatedUser validatedUser, CancellationToken cancellationToken = default)
    {
        if (parObject is null)
            throw new ArgumentNullException(nameof(parObject));

        if (validatedUser is null)
            throw new ArgumentNullException(nameof(validatedUser));

        await GrantConsentAsync(parObject, validatedUser, parObject.AuthorizationDetails, parObject.Claims, cancellationToken);
    }

    public async Task SignedIn(ParObject parObject, CancellationToken cancellationToken = default)
    {
        await _authorizationRequestPersistenceService.UpdateAsync(parObject, update =>
        {
            update.WasUserReauthenticated = true;
        }, cancellationToken);
    }
}
