using FAPIServer.Authentication;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;
using FAPIServer.ResponseHandling;
using FAPIServer.Validation;
using FAPIServer.Validation.Contexts;

namespace FAPIServer.RequestHandling.Default;

public class PushedAuthorizationHandler : IPushedAuthorizationHandler
{
    private readonly IClientAuthenticator _clientAuthenticator;
    private readonly IPushedAuthorizationRequestValidator _requestValidator;
    private readonly IPushedAuthorizationResponseGenerator _responseGenerator;

    public PushedAuthorizationHandler(IClientAuthenticator clientAuthenticator,
        IPushedAuthorizationRequestValidator requestValidator,
        IPushedAuthorizationResponseGenerator responseGenerator)
    {
        _clientAuthenticator = clientAuthenticator;
        _requestValidator = requestValidator;
        _responseGenerator = responseGenerator;
    }

    public async Task<PushedAuthorizationHandlerResult> HandleAsync(PushedAuthorizationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var authResult = await _clientAuthenticator.AuthenticateAsync(
            new ClientAuthenticationContext(context.AuthRequest, context.RequestedUri), cancellationToken);

        if (!authResult.IsAuthenticated)
            return new(authResult.Error, authResult.FailureMessage);

        var validationResult = await _requestValidator.ValidateAsync(
            new PushedAuthorizationRequestValidationContext(context.Request, authResult.Client), cancellationToken);

        if (!validationResult.IsValid)
            return new(validationResult.Error, validationResult.FailureMessage);

        var response = await _responseGenerator.GenerateAsync(validationResult.ValidatedRequest, cancellationToken);
        return new(response);
    }
}
