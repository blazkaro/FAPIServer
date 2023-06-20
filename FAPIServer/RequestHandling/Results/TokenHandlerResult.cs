using FAPIServer.ResponseHandling.Models;

namespace FAPIServer.RequestHandling.Results;

public class TokenHandlerResult : HandlerResultBase
{
    public TokenHandlerResult(Error? error) : base(error)
    {
    }

    public TokenHandlerResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public TokenHandlerResult(TokenResponse tokenResponse)
    {
        Success = true;
        TokenResponse = tokenResponse ?? throw new ArgumentNullException(nameof(tokenResponse));
    }

    public TokenResponse TokenResponse { get; init; }
}
