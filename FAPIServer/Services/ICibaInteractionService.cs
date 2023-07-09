using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Services;

public interface ICibaInteractionService
{
    Task GrantConsentAsync(CibaObject cibaObject, CancellationToken cancellationToken = default);
    Task GrantConsentAsync(CibaObject cibaObject, IEnumerable<AuthorizationDetail>? authorizationDetails, IEnumerable<string>? claims,
        CancellationToken cancellationToken = default);

    Task DenyConsentAsync(CibaObject cibaObject, CancellationToken cancellationToken = default);
}
