using FAPIServer.Extensions;
using FAPIServer.RequestHandling.Requests;
using FAPIServer.Services;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Models;
using FAPIServer.Validation.Results;
using System.Text.Json;

namespace FAPIServer.Validation.Default;

public class AuthorizationRequestValidator : IAuthorizationRequestValidator
{
    private readonly IClientStore _clientStore;
    private readonly IPushedAuthorizationRequestValidator _pushedAuthorizationRequestValidator;
    private readonly IAuthorizationRequestPersistenceService _authorizationRequestPersistenceService;

    public AuthorizationRequestValidator(IClientStore clientStore,
        IPushedAuthorizationRequestValidator pushedAuthorizationRequestValidator,
        IAuthorizationRequestPersistenceService authorizationRequestPersistenceService)
    {
        _clientStore = clientStore;
        _pushedAuthorizationRequestValidator = pushedAuthorizationRequestValidator;
        _authorizationRequestPersistenceService = authorizationRequestPersistenceService;
    }

    public async Task<AuthorizationRequestValidationResult> ValidateAsync(AuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (request.ClientId.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("client_id"));

        if (request.RequestUri.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("request_uri"));

        if (!request.RequestUri.StartsWith(Constants.RequestUriUrn))
            return new(Error.InvalidRequest, $"The 'request_uri' must starts with '{Constants.RequestUriUrn}'");

        var client = await _clientStore.FindEnabledByClientIdAsync(request.ClientId, cancellationToken);
        if (client == null || !client.AllowedGrantTypes.Contains(Constants.SupportedGrantTypes.AuthorizationCode))
            return new(Error.UnauthorizedClient, "The client not found or is not authorized");

        var parObject = await _authorizationRequestPersistenceService.ReadAsync(request, cancellationToken);
        if (parObject == null || (!parObject.HasBeenActivated && parObject.HasExpired()))
            return new(Error.InvalidRequestUri, "The 'request_uri' not found, has been consumed or has expired");

        // Re-validate so we can detect client and server policy changes.
        // By re-validating we can detect every change and return appropriate error if necessary
        var revalidationResult = await Revalidate(parObject, client, cancellationToken);
        if (!revalidationResult.IsValid)
            return new(revalidationResult.Error, revalidationResult.FailureMessage);

        return new(
            new ValidatedAuthorizationRequest(request, client, parObject, revalidationResult.ValidatedRequest.AuthorizationDetailSchemas));
    }

    private async Task<PushedAuthorizationRequestValidationResult> Revalidate(ParObject parObject, Client client, CancellationToken cancellationToken)
    {
        var revalidationResult = await _pushedAuthorizationRequestValidator.ValidateAsync(
            new PushedAuthorizationRequestValidationContext(new PushedAuthorizationRequest
            {
                AuthorizationDetails = parObject.AuthorizationDetails.Any() ? JsonSerializer.Serialize(parObject.AuthorizationDetails) : null,
                Claims = parObject.Claims.ToSpaceDelimitedString(),
                RedirectUri = parObject.RedirectUri,
                State = parObject.State,
                Nonce = parObject.Nonce,
                CodeChallengeMethod = parObject.CodeChallengeMethod,
                CodeChallenge = parObject.CodeChallenge.Value,
                GrantId = parObject.RequestedGrant?.GrantId,
                GrantManagementAction = parObject.GrantManagementAction,
                DPoPPkh = parObject.DPoPPkh?.Value,
                Prompt = parObject.Prompt,
                MaxAge = parObject.MaxAge
            }, client), cancellationToken);

        return revalidationResult;
    }
}
