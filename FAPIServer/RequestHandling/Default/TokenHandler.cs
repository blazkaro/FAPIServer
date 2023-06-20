using FAPIServer.Authentication;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;
using FAPIServer.ResponseHandling;
using FAPIServer.Validation;
using FAPIServer.Validation.Contexts;

namespace FAPIServer.RequestHandling.Default;

public class TokenHandler : ITokenHandler
{
    private readonly IClientAuthenticator _clientAuthenticator;
    private readonly ITokenRequestValidator _requestValidator;
    private readonly ITokenResponseGenerator _responseGenerator;

    public TokenHandler(IClientAuthenticator clientAuthenticator,
        ITokenRequestValidator requestValidator,
        ITokenResponseGenerator responseGenerator)
    {
        _clientAuthenticator = clientAuthenticator;
        _requestValidator = requestValidator;
        _responseGenerator = responseGenerator;
    }

    public async Task<TokenHandlerResult> HandleAsync(TokenContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var authResult = await _clientAuthenticator.AuthenticateAsync(
            new ClientAuthenticationContext(context.AuthRequest, context.RequestedUri), cancellationToken);

        if (!authResult.IsAuthenticated)
            return new(authResult.Error, authResult.FailureMessage);

        var validationContext = new TokenRequestValidationContext(context.Request,
            authResult.Client,
            new DPoPValidationParameters(context.RequestedUri, context.RequestedMethod));

        var validationResult = await _requestValidator.ValidateAsync(validationContext, cancellationToken);
        if (!validationResult.IsValid)
            return new(validationResult.Error, validationResult.FailureMessage);

        var response = await _responseGenerator.GenerateAsync(validationResult.ValidatedRequest, context.ResponseIssuer, cancellationToken);
        return new(response);
    }
}
