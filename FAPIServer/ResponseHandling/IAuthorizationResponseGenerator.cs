using FAPIServer.RequestHandling.Contexts;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Models;
using FAPIServer.Validation.Models;

namespace FAPIServer.ResponseHandling;

public interface IAuthorizationResponseGenerator
{
    Task<AuthorizationResponse> GenerateAsync(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest, Grant? similarGrant = null,
        CancellationToken cancellationToken = default);
    Task<AuthorizationResponse> GenerateAsync(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest, InteractionError interactionError,
        CancellationToken cancellationToken = default);
}
