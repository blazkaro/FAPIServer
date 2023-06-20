namespace FAPIServer.RequestHandling.Requests;

public class AuthorizationRequest : IRequest
{
    public virtual string ClientId { get; set; }
    public virtual string RequestUri { get; set; }
}
