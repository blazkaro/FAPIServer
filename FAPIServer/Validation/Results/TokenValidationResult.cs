namespace FAPIServer.Validation.Results;

public class TokenValidationResult<TPayload>
    where TPayload : class
{
    public TokenValidationResult(TPayload payload)
    {
        IsValid = true;
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public TokenValidationResult(string? failureMessage)
    {
        IsValid = false;
        FailureMessage = failureMessage;
    }

    public bool IsValid { get; init; }
    public TPayload Payload { get; init; }
    public string? FailureMessage { get; init; }

    public static TokenValidationResult<TPayload> MissingClaim(string claimName) => new($"The '{claimName}' claim is missing");
    public static TokenValidationResult<TPayload> InvalidClaim(string claimName) => new($"The '{claimName}' claim is invalid");
}
