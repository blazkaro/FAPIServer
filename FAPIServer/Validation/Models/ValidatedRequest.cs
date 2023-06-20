using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Models;

public abstract class ValidatedRequest<TRequest>
    where TRequest : class, IRequest
{
    protected ValidatedRequest(TRequest rawRequest, Client client)
    {
        RawRequest = rawRequest ?? throw new ArgumentNullException(nameof(rawRequest));
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public TRequest RawRequest { get; init; }
    public Client Client { get; init; }
}
