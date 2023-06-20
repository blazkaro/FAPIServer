using FAPIServer.Validation.Models;

namespace FAPIServer.Validation.Results;

public class AuthorizationRequestValidationResult : ValidationResultBase
{
    public AuthorizationRequestValidationResult(Error? error) : base(error)
    {
    }

    public AuthorizationRequestValidationResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public AuthorizationRequestValidationResult(ValidatedAuthorizationRequest validatedRequest)
    {
        IsValid = true;
        ValidatedRequest = validatedRequest ?? throw new ArgumentNullException(nameof(validatedRequest));
    }

    public ValidatedAuthorizationRequest ValidatedRequest { get; init; }
}
