using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Services;

public interface IGrantManager
{
    Task<Grant> CreateAsync(string clientId, string subject, IEnumerable<AuthorizationDetail> authorizationDetails, IEnumerable<string> claims,
        CancellationToken cancellationToken = default);

    Task MergeAsync(Grant grant, IEnumerable<AuthorizationDetail> authorizationDetails, IEnumerable<string> claims, CancellationToken cancellationToken = default);
    Task ReplaceAsync(Grant grant, IEnumerable<AuthorizationDetail> authorizationDetails, IEnumerable<string> claims, CancellationToken cancellationToken = default);
    Task RevokeAsync(string grantId, CancellationToken cancellationToken = default);
}
