using FAPIServer.Extensions;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;
using FAPIServer.ResponseHandling;
using FAPIServer.Services;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation;
using FAPIServer.Validation.Models;
using Microsoft.Extensions.Options;

namespace FAPIServer.RequestHandling.Default;

public class AuthorizationHandler : IAuthorizationHandler
{
    private readonly IAuthorizationRequestValidator _requestValidator;
    private readonly IAuthorizationResponseGenerator _responseGenerator;
    private readonly IGrantStore _grantStore;
    private readonly FapiOptions _options;
    private readonly IAuthorizationRequestPersistenceService _authorizationRequestPersistenceService;

    public AuthorizationHandler(IAuthorizationRequestValidator requestValidator,
        IAuthorizationResponseGenerator responseGenerator,
        IGrantStore grantStore,
        IOptionsMonitor<FapiOptions> options,
        IAuthorizationRequestPersistenceService authorizationRequestPersistenceService)
    {
        _requestValidator = requestValidator;
        _responseGenerator = responseGenerator;
        _grantStore = grantStore;
        _options = options.CurrentValue;
        _authorizationRequestPersistenceService = authorizationRequestPersistenceService;
    }

    public async Task<AuthorizationHandlerResult> HandleAsync(AuthorizationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var validationResult = await _requestValidator.ValidateAsync(context.Request, cancellationToken);
        if (!validationResult.IsValid)
            return new(validationResult.Error, validationResult.FailureMessage);

        var validatedRequest = validationResult.ValidatedRequest;

        if (!validatedRequest.ParObject.HasBeenActivated)
            await _authorizationRequestPersistenceService.PersistAsync(validatedRequest.ParObject, cancellationToken);

        if ((validatedRequest.ParObject.Prompt == Constants.PromptTypes.Login && !validatedRequest.ParObject.WasUserReauthenticated)
            || context.User is null || context.User.GetSubject().IsNullOrEmpty())
            return AuthorizationHandlerResult.AskForAuthentication();

        // Handle max_age
        {
            var authTime = context.User.GetAuthTime();
            if (!authTime.HasValue)
                throw new InvalidOperationException("The 'auth_time' claim has invalid format or is null. It must be DateTime");

            var offset = DateTime.UtcNow - authTime.Value;
            if (offset.Seconds >= validatedRequest.ParObject.MaxAge && !validatedRequest.ParObject.WasUserReauthenticated)
                return AuthorizationHandlerResult.AskForAuthentication();
        }

        context.SetValidUser();

        // Redirect with access_denied error if user explicitly denied access
        if (validatedRequest.ParObject.AccessDenied)
            return await SendInteractionError(context, validatedRequest, InteractionError.AccessDenied, cancellationToken);

        return validatedRequest.ParObject.GrantManagementAction switch
        {
            Constants.GrantManagementActions.Create => await HandleCreateAction(context, validatedRequest, cancellationToken),
            Constants.GrantManagementActions.Merge => await HandleMergeAction(context, validatedRequest, cancellationToken),
            Constants.GrantManagementActions.Replace => await HandleReplaceAction(context, validatedRequest, cancellationToken),
            _ => await HandleDefault(context, validatedRequest, cancellationToken)
        };
    }

    private async Task<AuthorizationHandlerResult> HandleCreateAction(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest,
        CancellationToken cancellationToken)
    {
        if (!validatedRequest.ParObject.WasConsentPageShown)
            return AuthorizationHandlerResult.AskForConsent();

        // If true, the user didn't gave consent
        if (validatedRequest.ParObject.Grant is null)
            return await SendInteractionError(context, validatedRequest, InteractionError.AccessDenied, cancellationToken);

        // Probably impossible to happen, but check it anyway
        if (validatedRequest.ParObject.Grant.Subject != context.GetValidUser().Subject)
            return new(Error.InvalidGrantId, "The grant was created but assigned to different user. More than 1 user wants to use the same request URI");

        return new(await _responseGenerator.GenerateAsync(context, validatedRequest, cancellationToken: cancellationToken));
    }

    private async Task<AuthorizationHandlerResult> HandleMergeAction(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest,
        CancellationToken cancellationToken)
    {
        if (validatedRequest.ParObject.Grant is null)
            throw new InvalidOperationException($"The {nameof(validatedRequest.ParObject.Grant)} cannot be null here");

        if (validatedRequest.ParObject.Grant.Subject != context.GetValidUser().Subject)
            return new(Error.InvalidGrantId, "The requested grant subject is different than authenticated user");

        if (!validatedRequest.ParObject.WasConsentPageShown)
        {
            if (_options.AlwaysRequireInteractionWhenMergingGrant)
            {
                return AuthorizationHandlerResult.AskForConsent();
            }
            else
            {
                if (IsFreshConsentRequired(validatedRequest)
                    || !IsGrantAllowingSameOrMoreAccessComparedToRequested(validatedRequest.ParObject.Grant, validatedRequest))
                    return AuthorizationHandlerResult.AskForConsent();

                return new(await _responseGenerator.GenerateAsync(context, validatedRequest, cancellationToken: cancellationToken));
            }
        }

        // The consent was shown, so use requested grant because in case of merge we  existing, not creating a new one.
        // So the requested grant will be d (merged) grant
        return new(await _responseGenerator.GenerateAsync(context, validatedRequest, cancellationToken: cancellationToken));
    }

    private async Task<AuthorizationHandlerResult> HandleReplaceAction(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest,
        CancellationToken cancellationToken)
    {
        if (!validatedRequest.ParObject.WasConsentPageShown)
            return AuthorizationHandlerResult.AskForConsent();

        // It was validated whether grant exists, so if now it's null then user revoked it
        if (validatedRequest.ParObject.Grant is null)
            throw new InvalidOperationException($"The {nameof(validatedRequest.ParObject.Grant)} cannot be null here");

        if (validatedRequest.ParObject.Grant.Subject != context.GetValidUser().Subject)
            return new(Error.InvalidGrantId, "The requested grant subject is different than authenticated user");

        // In case of replace, we replace access given earlier in grant, but the grant is still the same (has the same id), so use requested grant
        return new(await _responseGenerator.GenerateAsync(context, validatedRequest, cancellationToken: cancellationToken));
    }

    private async Task<AuthorizationHandlerResult> HandleDefault(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest,
        CancellationToken cancellationToken)
    {
        if (!validatedRequest.ParObject.WasConsentPageShown)
        {
            if (IsFreshConsentRequired(validatedRequest))
            {
                return AuthorizationHandlerResult.AskForConsent();
            }
            // Required but doesn't have to be fresh
            else if (validatedRequest.Client.ConsentRequired)
            {
                // Search for similar
                var allGrants = await _grantStore.FindAllBySubjectAndClientId(context.GetValidUser().Subject, validatedRequest.Client.ClientId, cancellationToken);
                var similarGrant = allGrants.FirstOrDefault(p => IsGrantAllowingSameOrMoreAccessComparedToRequested(p, validatedRequest));
                if (similarGrant is null)
                    return AuthorizationHandlerResult.AskForConsent();

                // Similar found so use it
                return new(await _responseGenerator.GenerateAsync(context, validatedRequest, similarGrant, cancellationToken));
            }
            // Not required
            else
            {
                return new(await _responseGenerator.GenerateAsync(context, validatedRequest, cancellationToken: cancellationToken));
            }
        }
        else
        {
            return await HandleCreateAction(context, validatedRequest, cancellationToken);
        }
    }

    private async Task<AuthorizationHandlerResult> SendInteractionError(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest,
        InteractionError interactionError, CancellationToken cancellationToken)
    {
        return new(await _responseGenerator.GenerateAsync(context, validatedRequest, interactionError, cancellationToken));
    }

    private static bool IsFreshConsentRequired(ValidatedAuthorizationRequest validatedRequest)
    {
        var openId = validatedRequest.ParObject.AuthorizationDetails.SingleOrDefault(p => p.Type == Constants.BuiltInAuthorizationDetails.OpenId.Type);
        if ((openId is not null && openId.Actions.ContainsKey(Constants.BuiltInAuthorizationDetails.OpenId.Actions.OfflineAccess))
            || validatedRequest.AuthorizationDetailSchemas.Any(p => !p.IsReusable))
            return true;

        return false;
    }

    private static bool IsGrantAllowingSameOrMoreAccessComparedToRequested(Grant grant, ValidatedAuthorizationRequest validatedRequest)
    {
        var claimsNotGranted = validatedRequest.ParObject.Claims.Except(grant.Claims);

        if (!claimsNotGranted.Any() || !validatedRequest.ParObject.AuthorizationDetails.IsIncludedIn(grant.AuthorizationDetails))
            return false;
        
        return true;
    }
}
