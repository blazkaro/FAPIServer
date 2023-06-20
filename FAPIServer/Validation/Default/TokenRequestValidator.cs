using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Models;
using FAPIServer.Validation.Results;
using NSec.Cryptography;
using System.Text.Json;

namespace FAPIServer.Validation.Default;

public class TokenRequestValidator : ITokenRequestValidator
{
    private readonly IAuthorizationCodeStore _authorizationCodeStore;
    private readonly IDPoPProofValidator _dpopProofValidator;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IResourceValidator _resourceValidator;

    public TokenRequestValidator(IAuthorizationCodeStore authorizationCodeStore,
        IDPoPProofValidator dpopProofValidator,
        IRefreshTokenStore refreshTokenStore,
        IResourceValidator resourceValidator)
    {
        _authorizationCodeStore = authorizationCodeStore;
        _dpopProofValidator = dpopProofValidator;
        _refreshTokenStore = refreshTokenStore;
        _resourceValidator = resourceValidator;
    }

    public async Task<TokenRequestValidationResult> ValidateAsync(TokenRequestValidationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (context.Request.GrantType.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("grant_type"));

        if (!Constants.SupportedGrantTypes.Types.Contains(context.Request.GrantType))
            return new(Error.UnsupportedGrantType, ErrorDescriptions.NotSupportedValue(context.Request.GrantType, "grant_type"));

        if (!context.Client.AllowedGrantTypes.Contains(context.Request.GrantType))
            return new(Error.UnauthorizedClient, ErrorDescriptions.UnauthorizedClient);

        return context.Request.GrantType switch
        {
            Constants.SupportedGrantTypes.AuthorizationCode => await ValidateAgainstAuthorizationCodeGrant(context, cancellationToken),
            Constants.SupportedGrantTypes.ClientCredentials => await ValidateAgainstClientCredentialsGrant(context, cancellationToken),
            Constants.SupportedGrantTypes.RefreshToken => await ValidateAgainstRefreshTokenGrant(context, cancellationToken)
            // No need for default case, because it was validated whether requested grant type is supported
        };
    }

    private async Task<TokenRequestValidationResult> ValidateAgainstAuthorizationCodeGrant(TokenRequestValidationContext context, CancellationToken cancellationToken)
    {
        if (context.Request.Code.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("code"));

        if (context.Request.CodeVerifier.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("code_verifier"));

        if (context.Request.RedirectUri.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("redirect_uri"));

        if (!Base64UrlEncoder.Validate(context.Request.CodeVerifier, out _))
            return new(Error.InvalidRequest, "The 'code_verifier' must be Base64-Url encoded string");

        var authorizationCode = await _authorizationCodeStore.FindByCodeAndClientIdAsync(context.Request.Code, context.Client.ClientId, cancellationToken);
        if (authorizationCode == null || authorizationCode.HasExpired())
            return new(Error.InvalidGrant, "The authorization code not found or has expired");

        if (!HashAlgorithm.Sha256.Verify(Base64UrlEncoder.Decode(context.Request.CodeVerifier), authorizationCode.CodeChallenge.Decode()))
            return new(Error.InvalidGrant, "The 'code_verifier' is invalid");

        if (!context.Request.RedirectUri.Equals(authorizationCode.RedirectUri, StringComparison.OrdinalIgnoreCase))
            return new(Error.InvalidGrant, "The 'redirect_uri' is invalid");

        if (authorizationCode.DPoPPkh is not null)
        {
            if (context.Request.DPoP.IsNullOrEmpty())
                return new(Error.InvalidDPoPProof, "The DPoP proof is required");

            context.DPoPValidationParameters.ValidPkh = authorizationCode.DPoPPkh.Decode();
        }

        if (!context.Request.DPoP.IsNullOrEmpty())
        {
            var validationResult = _dpopProofValidator.Validate(context.Request.DPoP, context.DPoPValidationParameters);
            if (!validationResult.IsValid)
                return new(Error.InvalidDPoPProof, validationResult.FailureMessage);
        }

        return new(new ValidatedTokenRequest(context.Request, context.Client, authorizationCode));
    }

    private async Task<TokenRequestValidationResult> ValidateAgainstClientCredentialsGrant(TokenRequestValidationContext context, CancellationToken cancellationToken)
    {
        var resourcesValidationResult = await _resourceValidator.ValidateAsync(new ResourceValidationContext(context.Client,
            null, context.Request.AuthorizationDetails, null), cancellationToken);

        if (!resourcesValidationResult.IsValid)
            return new(resourcesValidationResult.Error, resourcesValidationResult.FailureMessage);

        if (!context.Request.DPoP.IsNullOrEmpty())
        {
            var validationResult = _dpopProofValidator.Validate(context.Request.DPoP, context.DPoPValidationParameters);
            if (!validationResult.IsValid)
                return new(Error.InvalidDPoPProof, validationResult.FailureMessage);
        }

        // Override because using client_credentials grant the 'openid' authorization detail cannot be requested
        context.Request.AuthorizationDetails = JsonSerializer.Serialize(
            resourcesValidationResult.AuthorizationDetails.ExceptBy(new string[] { Constants.BuiltInAuthorizationDetails.OpenId.Type }, by => by.Type));

        return new(new ValidatedTokenRequest(context.Request, context.Client));
    }

    private async Task<TokenRequestValidationResult> ValidateAgainstRefreshTokenGrant(TokenRequestValidationContext context, CancellationToken cancellationToken)
    {
        if (context.Request.RefreshToken.IsNullOrEmpty())
            return new(Error.InvalidRequest, ErrorDescriptions.MissingParameter("refresh_token"));

        var refreshToken = await _refreshTokenStore.FindByTokenAsync(context.Request.RefreshToken, cancellationToken);
        if (refreshToken == null || refreshToken.HasExpired() || refreshToken.Grant is null)
            return new(Error.InvalidGrant, "The refresh token not found, has been revoked or expired");

        if (!context.Request.AuthorizationDetailsTypes.IsNullOrEmpty()
            && !context.Request.AuthorizationDetailsTypes.FromSpaceDelimitedString().All(refreshToken.AuthorizationDetails.Select(p => p.Type).Contains))
            return new(Error.InvalidAuthorizationDetaiTypes, "The requested authorization detail types are not granted by provided refresh token");

        if (!context.Request.Claims.IsNullOrEmpty() && !context.Request.Claims.FromSpaceDelimitedString().All(refreshToken.Claims.Contains))
            return new(Error.InvalidClaims, "The requested claims are not granted by provided refresh token");

        if (!context.Request.DPoP.IsNullOrEmpty())
        {
            var validationResult = _dpopProofValidator.Validate(context.Request.DPoP, context.DPoPValidationParameters);
            if (!validationResult.IsValid)
                return new(Error.InvalidDPoPProof, validationResult.FailureMessage);
        }

        return new(new ValidatedTokenRequest(context.Request, context.Client, refreshToken));
    }
}
