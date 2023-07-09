using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Contexts;

public class CibaRequestValidationContext
{
    public CibaRequestValidationContext(CibaRequest request, Client client, string tokenIssuer)
    {
        if (string.IsNullOrEmpty(tokenIssuer))
            throw new ArgumentException($"'{nameof(tokenIssuer)}' cannot be null or empty.", nameof(tokenIssuer));

        Request = request ?? throw new ArgumentNullException(nameof(request));
        Client = client ?? throw new ArgumentNullException(nameof(client));
        TokenIssuer = tokenIssuer;
    }

    public CibaRequest Request { get; set; }
    public Client Client { get; set; }
    public string TokenIssuer { get; set; }
}
