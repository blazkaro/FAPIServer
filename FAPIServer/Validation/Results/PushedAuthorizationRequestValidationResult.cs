using FAPIServer.Validation.Models;

namespace FAPIServer.Validation.Results;

public class PushedAuthorizationRequestValidationResult : ValidationResultBase
{
    public PushedAuthorizationRequestValidationResult(Error? error) : base(error)
    {
    }

    public PushedAuthorizationRequestValidationResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public PushedAuthorizationRequestValidationResult(ValidatedPushedAuthorizationRequest validatedRequest)
    {
        IsValid = true;
        ValidatedRequest = validatedRequest ?? throw new ArgumentNullException(nameof(validatedRequest));
    }

    public ValidatedPushedAuthorizationRequest ValidatedRequest { get; init; }
}
