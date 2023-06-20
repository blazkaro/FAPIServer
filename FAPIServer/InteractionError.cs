namespace FAPIServer;

public enum InteractionError
{
    AccessDenied,
    LoginRequired,
    ConsentRequired
}

public static class InteractionErrorExtensions
{
    public static string ToSnakeCase(this InteractionError interactionError)
    {
        return interactionError switch
        {
            InteractionError.AccessDenied => "access_denied",
            InteractionError.LoginRequired => "login_required",
            InteractionError.ConsentRequired => "consent_required",
            _ => throw new NotSupportedException("Given interaction error is not supported")
        };
    }
}