using FAPIServer.RequestHandling.Requests;

namespace FAPIServer.Authentication;

public class ClientAuthenticationContext
{
    public ClientAuthenticationContext(ClientAuthRequest authRequest, Uri validAudience)
    {
        AuthRequest = authRequest ?? throw new ArgumentNullException(nameof(authRequest));
        ValidAudience = validAudience ?? throw new ArgumentNullException(nameof(validAudience));
    }

    public ClientAuthRequest AuthRequest { get; set; }
    public Uri ValidAudience { get; set; }
}
