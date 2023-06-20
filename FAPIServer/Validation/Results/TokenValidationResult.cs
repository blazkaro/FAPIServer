namespace FAPIServer.Validation.Results;

public class TokenValidationResult
{
    public TokenValidationResult(IDictionary<string, object> payload)
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
    public IDictionary<string, object> Payload { get; init; }
    public string? FailureMessage { get; init; }

    public static TokenValidationResult MissingClaim(string claimName) => new($"The '{claimName}' claim is missing");
    public static TokenValidationResult InvalidClaim(string claimName) => new($"The '{claimName}' claim is invalid");
}
