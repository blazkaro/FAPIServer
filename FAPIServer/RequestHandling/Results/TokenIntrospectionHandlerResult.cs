using FAPIServer.ResponseHandling.Models;

namespace FAPIServer.RequestHandling.Results;

public class TokenIntrospectionHandlerResult : HandlerResultBase
{
    public TokenIntrospectionHandlerResult(Error? error) : base(error)
    {
    }

    public TokenIntrospectionHandlerResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public TokenIntrospectionHandlerResult(TokenIntrospectionResponse response)
    {
        Success = true;
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public TokenIntrospectionResponse Response { get; init; }
}
