using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Models;
using FAPIServer.Validation.Results;
using Microsoft.Extensions.Options;

namespace FAPIServer.Validation.Default;

public class PushedAuthorizationRequestValidator : IPushedAuthorizationRequestValidator
{
    private readonly FapiOptions _options;
    private readonly IGrantManagementValidator _grantManagementValidator;
    private readonly IResourceValidator _resourceValidator;

    public PushedAuthorizationRequestValidator(IOptionsMonitor<FapiOptions> options,
        IGrantManagementValidator grantManagementValidator,
        IResourceValidator resourceValidator)
    {
        _options = options.CurrentValue;
        _grantManagementValidator = grantManagementValidator;
        _resourceValidator = resourceValidator;
    }

    public async Task<PushedAuthorizationRequestValidationResult> ValidateAsync(PushedAuthorizationRequestValidationContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (!context.Client.AllowedGrantTypes.Contains(Constants.SupportedGrantTypes.AuthorizationCode))
            return new(Error.UnauthorizedClient, ErrorDescriptions.UnauthorizedClient);

        // Whether required parameters are present
        {
            var (IsValid, InvalidParamName) = ValidateAgainstRequiredParameters(context.Request);
            if (!IsValid) return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter(InvalidParamName));
        }

        // Whether requested parameters meets length restrictions
        {
            var (IsValid, InvalidParamName) = ValidateAgainstLengthRestrictions(context.Request);
            if (!IsValid)
                return new(Error.InvalidRequest, ErrorDescriptions.LengthRestrictionsUnsatisfied(InvalidParamName));
        }

        if (context.Request.CodeChallengeMethod != Constants.SupportedCodeChallengeMethods.S256)
            return new(Error.InvalidRequest, ErrorDescriptions.NotSupportedValue(context.Request.CodeChallengeMethod, "code_challenge_method"));

        // Whether code_challenge is Base64-Url encoded
        if (!Base64UrlEncoder.Validate(context.Request.CodeChallenge, out _))
            return new(Error.InvalidRequest, "The 'code_challenge' must be Base64-Url encoded string");

        // Whether code binding must be used
        if (context.Client.AuthorizationCodeBindingToDpopKeyRequired && context.Request.DPoPPkh.IsNullOrEmpty())
            return new(Error.InvalidRequest, "This client must use Authorization Code Binding To DPoP Key");

        // Whether dpop_pkh is Base64-Url encoded and has 32 bytes written
        if (!context.Request.DPoPPkh.IsNullOrEmpty() &&
            (!Base64UrlEncoder.Validate(context.Request.DPoPPkh, out int dpopPkhBytesWritten) || dpopPkhBytesWritten != 32))
            return new(Error.InvalidRequest, "The 'dpop_pkh' must be Base64-Url encoded string of SHA-256 hash");

        // Whether requested prompt value is supported
        if (!string.IsNullOrEmpty(context.Request.Prompt) && !Constants.SupportedPromptTypes.Types.Contains(context.Request.Prompt))
            return new(Error.InvalidRequest, ErrorDescriptions.NotSupportedValue(context.Request.Prompt, "prompt"));

        // Whether max_age is greater than 0
        if (context.Request.MaxAge.HasValue && context.Request.MaxAge.Value < 0)
            return new(Error.InvalidRequest, "The 'max_age' must be non-negative integer");

        Grant? requestedGrant = null;
        // Whether grant_id and grant_management_action are valid
        {
            var result = await _grantManagementValidator.ValidateAsync(
                new GrantManagementValidationContext(context.Client.ClientId, context.Request.GrantId, context.Request.GrantManagementAction),
                cancellationToken);

            if (!result.IsValid)
                return new(result.Error, result.FailureMessage);

            requestedGrant = result.Grant;
        }

        if (!context.Client.RedirectUris.Select(p => p.ToString()).Contains(context.Request.RedirectUri))
            return new(Error.InvalidRedirectUri, "The requested 'redirect_uri' is not valid for this client");

        // Validate requested resources
        var resourcesValidationResult = await _resourceValidator.ValidateAsync(new ResourceValidationContext(context.Client,
            context.Request.GrantManagementAction,
            context.Request.AuthorizationDetails,
            context.Request.Claims,
            requestedGrant), cancellationToken);

        if (!resourcesValidationResult.IsValid)
            return new(resourcesValidationResult.Error, resourcesValidationResult.FailureMessage);

        if (context.Request.GrantManagementAction != Constants.SupportedGrantManagementActions.Merge
            && !resourcesValidationResult.Claims.Contains(Constants.BuiltInClaims.Subject))
            return new(Error.InvalidClaims, $"The '{Constants.BuiltInClaims.Subject}' must be requested");

        return new(new ValidatedPushedAuthorizationRequest(context.Request,
            context.Client,
            resourcesValidationResult.AuthorizationDetails,
            resourcesValidationResult.InvolvedSchemas,
            resourcesValidationResult.Claims,
            requestedGrant));
    }

    private (bool IsValid, string InvalidParamName) ValidateAgainstRequiredParameters(PushedAuthorizationRequest request)
    {
        if (request.Claims.IsNullOrEmpty() && request.GrantManagementAction != Constants.SupportedGrantManagementActions.Merge)
            return new(false, "claims");

        if (request.RedirectUri.IsNullOrEmpty())
            return new(false, "redirect_uri");

        if (request.State.IsNullOrEmpty())
            return new(false, "state");

        if (request.Nonce.IsNullOrEmpty())
            return new(false, "nonce");

        if (request.CodeChallengeMethod.IsNullOrEmpty())
            return new(false, "code_challenge_method");

        if (request.CodeChallenge.IsNullOrEmpty())
            return new(false, "code_challenge");

        if (request.GrantManagementAction.IsNullOrEmpty() && _options.GrantManagementActionRequired)
            return new(false, "grant_management_action");

        return new(true, string.Empty);
    }

    private (bool IsValid, string InvalidParamName) ValidateAgainstLengthRestrictions(PushedAuthorizationRequest request)
    {
        if (request.State.Length < _options.InputRestrictions.MinStateLength || request.State.Length > _options.InputRestrictions.MaxStateLength)
            return new(false, "state");

        if (request.Nonce.Length < _options.InputRestrictions.MinNonceLength || request.Nonce.Length > _options.InputRestrictions.MaxNonceLength)
            return new(false, "nonce");

        if (request.CodeChallenge.Length < _options.InputRestrictions.MinCodeChallengeLength || request.CodeChallenge.Length > _options.InputRestrictions.MaxCodeChallengeLength)
            return new(false, "code_challenge");

        return new(true, string.Empty);
    }
}
