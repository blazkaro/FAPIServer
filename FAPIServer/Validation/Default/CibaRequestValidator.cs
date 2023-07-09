using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.Services;
using FAPIServer.Storage.Models;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Models;
using FAPIServer.Validation.Results;
using Microsoft.Extensions.Options;

namespace FAPIServer.Validation.Default;

public class CibaRequestValidator : ICibaRequestValidator
{
    private readonly FapiOptions _options;
    private readonly IUserService _userService;
    private readonly IIdTokenValidator _idTokenValidator;
    private readonly IGrantManagementValidator _grantManagementValidator;
    private readonly IResourceValidator _resourceValidator;
    private readonly ICibaUserNotificationService _cibaUserNotificationService;

    public CibaRequestValidator(IOptionsMonitor<FapiOptions> options,
        IUserService userService,
        IIdTokenValidator idTokenValidator,
        IGrantManagementValidator grantManagementValidator,
        IResourceValidator resourceValidator,
        ICibaUserNotificationService cibaUserNotificationService)
    {
        _options = options.CurrentValue;
        _userService = userService;
        _idTokenValidator = idTokenValidator;
        _grantManagementValidator = grantManagementValidator;
        _resourceValidator = resourceValidator;
        _cibaUserNotificationService = cibaUserNotificationService;
    }

    public async Task<CibaRequestValidationResult> ValidateAsync(CibaRequestValidationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        // Whether client is allowed to use CIBA grant type
        if (!context.Client.AllowedGrantTypes.Contains(Constants.GrantTypes.Ciba))
            return new(Error.UnauthorizedClient, ErrorDescriptions.UnauthorizedClient);

        // Whether claims is present
        if (context.Request.Claims.IsNullOrEmpty() && context.Request.GrantManagementAction != Constants.GrantManagementActions.Merge)
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("claims"));

        // Whether client notification token is present when client uses Ping mode
        if (context.Client.CibaMode == Constants.CibaModes.Ping && context.Request.ClientNotificationToken.IsNullOrEmpty())
            return new(Error.InvalidRequest, "The 'client_notification_token' is required with Ping mode");

        // Whether user id hint exists and is unambiguous
        if ((context.Request.IdTokenHint.IsNullOrEmpty() && context.Request.LoginHint.IsNullOrEmpty())
            || (!context.Request.IdTokenHint.IsNullOrEmpty() && !context.Request.LoginHint.IsNullOrEmpty()))
            return new(Error.InvalidRequest, "The request must use at least and at max one of these parameters: 'id_token_hint' and 'login_hint'");

        // Whether dpop_pkh is Base64-Url encoded and has 32 bytes written
        if (!context.Request.DPoPPkh.IsNullOrEmpty() &&
            (!Base64UrlEncoder.Validate(context.Request.DPoPPkh, out int dpopPkhBytesWritten) || dpopPkhBytesWritten != 32))
            return new(Error.InvalidRequest, "The 'dpop_pkh' must be Base64-Url encoded string of SHA-256 hash");

        // Whether binding message isn't too long
        if (!context.Request.BindingMessage.IsNullOrEmpty() && context.Request.BindingMessage.Length > _options.InputRestrictions.MaxCibaBindingMessageLength)
            return new(Error.InvalidBindingMessage, $"The message is too long");

        Grant? requestedGrant;
        // Whether grant_id and grant_management_action are valid
        {
            var grantManagementValidationResult = await _grantManagementValidator.ValidateAsync(
                new GrantManagementValidationContext(context.Client.ClientId,
                context.Request.GrantId, context.Request.GrantManagementAction), cancellationToken);

            if (!grantManagementValidationResult.IsValid)
                return new(grantManagementValidationResult.Error, grantManagementValidationResult.FailureMessage);

            requestedGrant = grantManagementValidationResult.Grant;
        }

        // Validate requested resources
        var resourcesValidationResult = await _resourceValidator.ValidateAsync(new ResourceValidationContext(context.Client,
            context.Request.GrantManagementAction, context.Request.AuthorizationDetails,
            context.Request.Claims, Constants.GrantTypes.Ciba, requestedGrant), cancellationToken);

        if (!resourcesValidationResult.IsValid)
            return new(resourcesValidationResult.Error, resourcesValidationResult.FailureMessage);

        // Retrieve user id from hint
        string subject;
        {
            if (!context.Request.IdTokenHint.IsNullOrEmpty())
            {
                var validationResult = await _idTokenValidator.ValidateAsync(context.Request.IdTokenHint,
                    context.TokenIssuer, context.Client.ClientId, cancellationToken);

                if (!validationResult.IsValid)
                    return new(Error.UnknownUserId, $"The provided identity token is not valid. {validationResult.FailureMessage}");

                subject = validationResult.Payload.Subject;
            }
            else
            {
                subject = context.Request.LoginHint!;
            }
        }

        // Get user-ciba context, validate user code
        var userCibaCtx = await _userService.GetCibaContextAsync(subject, context.Request.UserCode, cancellationToken);
        if (userCibaCtx.RequireUserCode && context.Request.UserCode.IsNullOrEmpty())
            return new(Error.MissingUserCode, "The user code is required but not present");

        if (!userCibaCtx.IsUserCodeValid)
            return new(Error.InvalidUserCode, "The user code is invalid");

        var validatedRequest = new ValidatedCibaRequest(context.Request, context.Client,
            resourcesValidationResult.AuthorizationDetails, resourcesValidationResult.Claims, subject, requestedGrant);

        await _cibaUserNotificationService.SendNotificationAsync(validatedRequest, cancellationToken);

        return new(validatedRequest);
    }
}
