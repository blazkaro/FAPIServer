using FAPIServer.Storage.Models;
using System.Security.Claims;

namespace FAPIServer.Authentication;

public class ClientAuthenticationResult
{
    public ClientAuthenticationResult(Client client, IEnumerable<Claim> assertionPayload)
    {
        IsAuthenticated = true;
        Client = client ?? throw new ArgumentNullException(nameof(client));
        AssertionPayload = assertionPayload ?? throw new ArgumentNullException(nameof(assertionPayload));
    }

    public ClientAuthenticationResult(Error? error)
    {
        IsAuthenticated = false;
        Error = error;
    }

    public ClientAuthenticationResult(Error? error, string? failureMessage) : this(error)
    {
        IsAuthenticated = false;
        FailureMessage = failureMessage;
    }

    public bool IsAuthenticated { get; init; }
    public Client Client { get; init; }
    public IEnumerable<Claim> AssertionPayload { get; init; }
    public Error? Error { get; init; }
    public string? FailureMessage { get; init; }
}
