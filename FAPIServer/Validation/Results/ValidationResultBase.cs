namespace FAPIServer.Validation.Results;

public abstract class ValidationResultBase
{
    public ValidationResultBase()
    {
        
    }

    protected ValidationResultBase(Error? error)
    {
        IsValid = false;
        Error = error;
    }

    protected ValidationResultBase(Error? error, string? failureMessage)
        : this(error)
    {
        IsValid = false;
        FailureMessage = failureMessage;
    }

    public bool IsValid { get; init; }
    public Error? Error { get; init; }
    public string? FailureMessage { get; init; }
}