namespace FAPIServer.RequestHandling.Requests;

public class PushedAuthorizationRequest : IRequest
{
    public virtual string? AuthorizationDetails { get; set; }
    public virtual string? Claims { get; set; }
    public virtual string RedirectUri { get; set; }
    public virtual string State { get; set; }
    public virtual string Nonce { get; set; }
    public virtual string CodeChallengeMethod { get; set; }
    public virtual string CodeChallenge { get; set; }
    public virtual string? GrantId { get; set; }
    public virtual string? GrantManagementAction { get; set; }
    public virtual string? DPoPPkh { get; set; }
    public virtual string? Prompt { get; set; }
    public virtual int? MaxAge { get; set; }
}
