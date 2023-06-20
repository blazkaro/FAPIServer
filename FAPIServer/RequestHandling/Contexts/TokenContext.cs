using FAPIServer.RequestHandling.Requests;

namespace FAPIServer.RequestHandling.Contexts;

public class TokenContext
{
    public TokenContext(ClientAuthRequest authRequest, TokenRequest request, Uri requestedUri, string requestedMethod, string responseIssuer)
    {
        if (string.IsNullOrEmpty(requestedMethod))
            throw new ArgumentException($"'{nameof(requestedMethod)}' cannot be null or empty.", nameof(requestedMethod));

        if (string.IsNullOrEmpty(responseIssuer))
            throw new ArgumentException($"'{nameof(responseIssuer)}' cannot be null or empty.", nameof(responseIssuer));

        AuthRequest = authRequest ?? throw new ArgumentNullException(nameof(authRequest));
        Request = request ?? throw new ArgumentNullException(nameof(request));
        RequestedUri = requestedUri ?? throw new ArgumentNullException(nameof(requestedUri));
        RequestedMethod = requestedMethod;
        ResponseIssuer = responseIssuer;
    }

    public ClientAuthRequest AuthRequest { get; set; }
    public TokenRequest Request { get; set; }
    public Uri RequestedUri { get; set; }
    public string RequestedMethod { get; set; }
    public string ResponseIssuer { get; set; }
}
