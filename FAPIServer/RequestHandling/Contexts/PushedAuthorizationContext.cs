using FAPIServer.RequestHandling.Requests;

namespace FAPIServer.RequestHandling.Contexts;

public class PushedAuthorizationContext
{
    public PushedAuthorizationContext(ClientAuthRequest authRequest, PushedAuthorizationRequest request, Uri requestedUri)
    {
        AuthRequest= authRequest ?? throw new ArgumentNullException(nameof(authRequest));
        Request = request ?? throw new ArgumentNullException(nameof(request));
        RequestedUri = requestedUri ?? throw new ArgumentNullException(nameof(requestedUri));
    }

    public ClientAuthRequest AuthRequest { get; set; }
    public PushedAuthorizationRequest Request { get; set; }
    public Uri RequestedUri { get; set; }
}
