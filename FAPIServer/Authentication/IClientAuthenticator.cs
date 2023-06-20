namespace FAPIServer.Authentication;

public interface IClientAuthenticator
{
    Task<ClientAuthenticationResult> AuthenticateAsync(ClientAuthenticationContext context, CancellationToken cancellationToken = default);
}
