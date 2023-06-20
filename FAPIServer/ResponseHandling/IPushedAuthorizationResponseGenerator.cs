using FAPIServer.ResponseHandling.Models;
using FAPIServer.Validation.Models;

namespace FAPIServer.ResponseHandling;

public interface IPushedAuthorizationResponseGenerator
{
    Task<PushedAuthorizationResponse> GenerateAsync(ValidatedPushedAuthorizationRequest validatedRequest, CancellationToken cancellationToken = default);
}
