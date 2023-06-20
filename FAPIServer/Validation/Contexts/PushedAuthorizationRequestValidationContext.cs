using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Contexts;

public class PushedAuthorizationRequestValidationContext
{
    public PushedAuthorizationRequestValidationContext(PushedAuthorizationRequest request, Client client)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public PushedAuthorizationRequest Request { get; set; }
    public Client Client { get; set; }
}
