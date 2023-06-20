using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Models;

namespace FAPIServer.Services;

public interface IInteractionService
{
    Task GrantConsentAsync(ParObject parObject, ValidatedUser user, IEnumerable<AuthorizationDetail>? grantedAuthorizationDetails, IEnumerable<string>? grantedClaims,
        CancellationToken cancellationToken = default);

    Task DenyConsentAsync(ParObject parObject, CancellationToken cancellationToken = default);
    Task SignedIn(ParObject parObject, CancellationToken cancellationToken = default);
}
