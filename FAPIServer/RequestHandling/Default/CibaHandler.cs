using FAPIServer.Authentication;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;
using FAPIServer.ResponseHandling;
using FAPIServer.Validation;
using FAPIServer.Validation.Contexts;

namespace FAPIServer.RequestHandling.Default;

public class CibaHandler : ICibaHandler
{
    private readonly IClientAuthenticator _clientAuthenticator;
    private readonly ICibaRequestValidator _requestValidator;
    private readonly ICibaResponseGenerator _responseGenerator;

    public CibaHandler(IClientAuthenticator clientAuthenticator,
        ICibaRequestValidator requestValidator,
        ICibaResponseGenerator responseGenerator)
    {
        _clientAuthenticator = clientAuthenticator;
        _requestValidator = requestValidator;
        _responseGenerator = responseGenerator;
    }

    public async Task<CibaHandlerResult> HandleAsync(CibaContext context, CancellationToken cancellationToken = default)
    {
        var authResult = await _clientAuthenticator.AuthenticateAsync(new ClientAuthenticationContext(context.AuthRequest, context.RequestedUri),
            cancellationToken);

        if (!authResult.IsAuthenticated)
            return new(authResult.Error, authResult.FailureMessage);

        var validationResult = await _requestValidator.ValidateAsync(
            new CibaRequestValidationContext(context.Request, authResult.Client, context.ValidTokenIssuer),
            cancellationToken);

        if (!validationResult.IsValid)
            return new(validationResult.Error, validationResult.FailureMessage);

        var resposne = await _responseGenerator.GenerateAsync(validationResult.ValidatedRequest, cancellationToken);
        return new CibaHandlerResult(resposne);
    }
}
