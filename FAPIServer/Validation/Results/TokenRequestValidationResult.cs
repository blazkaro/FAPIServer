using FAPIServer.Validation.Models;

namespace FAPIServer.Validation.Results;

public class TokenRequestValidationResult : ValidationResultBase
{
    public TokenRequestValidationResult(Error? error) : base(error)
    {
    }

    public TokenRequestValidationResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public TokenRequestValidationResult(ValidatedTokenRequest validatedRequest)
    {
        IsValid = true;
        ValidatedRequest = validatedRequest ?? throw new ArgumentNullException(nameof(validatedRequest));
    }

    public ValidatedTokenRequest ValidatedRequest { get; init; }
}
