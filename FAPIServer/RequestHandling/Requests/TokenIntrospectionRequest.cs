namespace FAPIServer.RequestHandling.Requests;

public class TokenIntrospectionRequest : IRequest
{
    public virtual string Token { get; set; }
}
