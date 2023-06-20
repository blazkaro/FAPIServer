namespace FAPIServer.RequestHandling.Requests;

public class TokenRevocationRequest : IRequest
{
    public virtual string Token { get; set; }
}
