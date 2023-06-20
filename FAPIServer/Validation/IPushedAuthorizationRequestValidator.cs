using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IPushedAuthorizationRequestValidator
{
    Task<PushedAuthorizationRequestValidationResult> ValidateAsync(PushedAuthorizationRequestValidationContext context, CancellationToken cancellationToken = default);
}
