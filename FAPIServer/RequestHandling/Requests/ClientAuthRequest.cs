namespace FAPIServer.RequestHandling.Requests;

public class ClientAuthRequest : IRequest
{
    public virtual string ClientId { get; set; }
    public virtual string ClientAssertionType { get; set; }
    public virtual string ClientAssertion { get; set; }

    public ClientAuthRequest GetAuthRequest() => this;
}
