using FAPIServer.RequestHandling.Requests;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IAuthorizationRequestValidator
{
    Task<AuthorizationRequestValidationResult> ValidateAsync(AuthorizationRequest request, CancellationToken cancellationToken = default);
}
