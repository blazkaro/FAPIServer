namespace FAPIServer.RequestHandling.Results;

public class TokenRevocationHandlerResult : HandlerResultBase
{
    public TokenRevocationHandlerResult()
    {
        
    }

    public TokenRevocationHandlerResult(Error? error) : base(error)
    {
    }

    public TokenRevocationHandlerResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }
}
