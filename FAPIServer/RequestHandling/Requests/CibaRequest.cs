namespace FAPIServer.RequestHandling.Requests;

public class CibaRequest : IRequest
{
    public virtual string AuthorizationDetails { get; set; }
    public virtual string Claims { get; set; }
    public virtual string? ClientNotificationToken { get; set; }
    public virtual string? IdTokenHint { get; set; }
    public virtual string? LoginHint { get; set; }
    public virtual string? BindingMessage { get; set; }
    public virtual string? UserCode { get; set; }
    public virtual string? GrantId { get; set; }
    public virtual string? GrantManagementAction { get; set; }
    public virtual string? DPoPPkh { get; set; }
}
