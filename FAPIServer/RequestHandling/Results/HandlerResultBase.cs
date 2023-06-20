namespace FAPIServer.RequestHandling.Results;

public abstract class HandlerResultBase
{
    protected HandlerResultBase()
    {

    }

    protected HandlerResultBase(Error? error)
    {
        Success = false;
        Error = error;
    }

    protected HandlerResultBase(Error? error, string? failureMessage) 
        : this(error)
    {
        FailureMessage = failureMessage;
    }

    public bool Success { get; init; }
    public Error? Error { get; init; }
    public string? FailureMessage { get; init; }
}
