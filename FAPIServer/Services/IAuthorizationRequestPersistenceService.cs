using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;

namespace FAPIServer.Services;

public interface IAuthorizationRequestPersistenceService
{
    /// <summary>
    /// Persists the <see cref="ParObject"/> and marks as activated
    /// </summary>
    /// <param name="parObject">The <see cref="ParObject"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The completed task</returns>
    Task PersistAsync(ParObject parObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves persisted <see cref="ParObject"/> by <see cref="AuthorizationRequest"/>.
    /// If some validation must be performed and retrieved <see cref="ParObject"/> is not valid, MUST return null
    /// </summary>
    /// <param name="request">The <see cref="AuthorizationRequest"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ParObject"/> if found and valid; otherwise, false</returns>
    Task<ParObject?> ReadAsync(AuthorizationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes persisted <see cref="ParObject"/>
    /// </summary>
    /// <param name="parObject">The <see cref="ParObject"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns></returns>
    Task RemoveAsync(ParObject parObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the persisted <see cref="ParObject"/>
    /// </summary>
    /// <param name="parObject">The <see cref="ParObject"/></param>
    /// <param name="update">Action which will be invoked on persisted <see cref="ParObject"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The completed task</returns>
    /// <exception cref="InvalidOperationException">When the <paramref name="parObject"/> is not persisted</exception>
    Task UpdateAsync(ParObject parObject, Action<ParObject> update, CancellationToken cancellationToken = default);
}
