using FAPIServer.ResponseHandling.Models;

namespace FAPIServer.RequestHandling.Results;

public class AuthorizationHandlerResult : HandlerResultBase
{
    public AuthorizationHandlerResult(Error? error)
        : base(error)
    {
    }

    public AuthorizationHandlerResult(Error? error, string? failureMessage)
        : base(error, failureMessage)
    {
    }

    public AuthorizationHandlerResult(AuthorizationResponse response)
    {
        Success = true;
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    private AuthorizationHandlerResult()
    {

    }

    public AuthorizationResponse Response { get; init; }
    public bool AuthenticationRequired { get; private init; }
    public bool ConsentRequired { get; private init; }

    public static AuthorizationHandlerResult AskForAuthentication()
        => new()
        {
            Success = true,
            AuthenticationRequired = true
        };

    public static AuthorizationHandlerResult AskForConsent()
        => new()
        {
            Success = true,
            ConsentRequired = true
        };
}
