using FAPIServer.ResponseHandling.Models;

namespace FAPIServer.RequestHandling.Results;

public class PushedAuthorizationHandlerResult : HandlerResultBase
{
    public PushedAuthorizationHandlerResult(Error? error) 
        : base(error)
    {
    }

    public PushedAuthorizationHandlerResult(Error? error, string? failureMessage) 
        : base(error, failureMessage)
    {
    }

    public PushedAuthorizationHandlerResult(PushedAuthorizationResponse response)
    {
        Success = true;
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public PushedAuthorizationResponse Response { get; init; }
}
