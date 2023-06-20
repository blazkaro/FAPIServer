namespace FAPIServer.RequestHandling.Requests;

public class TokenRequest : IRequest
{
    public virtual string GrantType { get; set; }
    public virtual string? Code { get; set; }
    public virtual string? CodeVerifier { get; set; }
    public virtual string? RedirectUri { get; set; }
    public virtual string? AuthorizationDetails { get; set; }
    public virtual string? AuthorizationDetailsTypes { get; set; }
    public virtual string? Claims { get; set; }
    public virtual string? RefreshToken { get; set; }
    public virtual string? DPoP { get; set; }
}
