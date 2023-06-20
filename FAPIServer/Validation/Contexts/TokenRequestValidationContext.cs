using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Contexts;

public class TokenRequestValidationContext
{
    public TokenRequestValidationContext(TokenRequest request, Client client, DPoPValidationParameters dPoPValidationParameters)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Client = client ?? throw new ArgumentNullException(nameof(client));
        DPoPValidationParameters = dPoPValidationParameters ?? throw new ArgumentNullException(nameof(dPoPValidationParameters));
    }

    public TokenRequest Request { get; set; }
    public Client Client { get; set; }
    public DPoPValidationParameters DPoPValidationParameters { get; set; }
}
