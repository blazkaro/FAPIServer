using FAPIServer.Validation.Models;

namespace FAPIServer.Validation.Results;

public class CibaRequestValidationResult : ValidationResultBase
{
    public CibaRequestValidationResult(Error? error) : base(error)
    {
    }

    public CibaRequestValidationResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public CibaRequestValidationResult(ValidatedCibaRequest validatedRequest)
    {
        IsValid = true;
        ValidatedRequest = validatedRequest;
    }

    public ValidatedCibaRequest ValidatedRequest { get; init; }
}
