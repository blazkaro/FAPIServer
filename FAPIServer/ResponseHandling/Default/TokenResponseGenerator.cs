using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.Models;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Services;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Models;
using NSec.Cryptography;
using Paseto;
using Paseto.Cryptography.Key;
using Paseto.Protocol;
using System.Text;

namespace FAPIServer.ResponseHandling.Default;

public class TokenResponseGenerator : ITokenResponseGenerator
{
    private readonly IAuthorizationCodeStore _authorizationCodeStore;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IIdTokenService _idTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ICibaObjectStore _cibaObjectStore;

    public TokenResponseGenerator(IAuthorizationCodeStore authorizationCodeStore,
        IAccessTokenService accessTokenService,
        IIdTokenService idTokenService,
        IRefreshTokenService refreshTokenService,
        ICibaObjectStore cibaObjectStore)
    {
        _authorizationCodeStore = authorizationCodeStore;
        _accessTokenService = accessTokenService;
        _idTokenService = idTokenService;
        _refreshTokenService = refreshTokenService;
        _cibaObjectStore = cibaObjectStore;
    }

    public async Task<TokenResponse> GenerateAsync(ValidatedTokenRequest validatedRequest, string responseIssuer, CancellationToken cancellationToken = default)
    {
        if (validatedRequest is null)
            throw new ArgumentNullException(nameof(validatedRequest));

        if (string.IsNullOrEmpty(responseIssuer))
            throw new ArgumentException($"'{nameof(responseIssuer)}' cannot be null or empty.", nameof(responseIssuer));

        return validatedRequest.RawRequest.GrantType switch
        {
            Constants.GrantTypes.AuthorizationCode => await GenerateForAuthorizationCodeGrant(validatedRequest, responseIssuer, cancellationToken),
            Constants.GrantTypes.ClientCredentials => await GenerateForClientCredentialsGrant(validatedRequest, responseIssuer, cancellationToken),
            Constants.GrantTypes.RefreshToken => await GenerateForRefreshTokenGrant(validatedRequest, responseIssuer, cancellationToken),
            Constants.GrantTypes.Ciba => await GenerateForCibaGrant(validatedRequest, responseIssuer, cancellationToken),
            _ => throw new NotSupportedException($"The '{validatedRequest.RawRequest.GrantType}' is not supported grant type")
        };
    }

    private async Task<TokenResponse> GenerateForAuthorizationCodeGrant(ValidatedTokenRequest validatedRequest, string responseIssuer, CancellationToken cancellationToken)
    {
        if (validatedRequest.AuthorizationCode is null)
            throw new ArgumentException($"The {nameof(validatedRequest.AuthorizationCode)} cannot be null here", nameof(validatedRequest));

        await _authorizationCodeStore.RemoveAsync(validatedRequest.AuthorizationCode.Code, cancellationToken);

        var utcNow = DateTime.UtcNow;
        var accessToken = await _accessTokenService.GenerateAsync(new AccessTokenPayload
        {
            Issuer = responseIssuer,
            Subject = validatedRequest.AuthorizationCode.Subject,
            NotBefore = utcNow,
            Expiration = utcNow.AddSeconds(validatedRequest.Client.AccessTokenLifetime.Seconds),
            Jti = Guid.NewGuid(),
            ClientId = validatedRequest.Client.ClientId,
            AuthorizationDetails = validatedRequest.AuthorizationCode.AuthorizationDetails,
            Claims = validatedRequest.AuthorizationCode.Claims.ToSpaceDelimitedString(),
            Cnf = new CnfClaim
            {
                Pkh = Base64UrlEncoder.Encode(SelectPaserkHash(validatedRequest))
            }
        }, cancellationToken);

        var idToken = await _idTokenService.GenerateAsync(responseIssuer, validatedRequest.AuthorizationCode, validatedRequest.Client.AccessTokenLifetime,
            cancellationToken);

        RefreshToken? refreshToken = null;
        if (ShallIssueRefreshToken(validatedRequest.AuthorizationCode.AuthorizationDetails))
            refreshToken = await _refreshTokenService.GenerateAsync(validatedRequest.AuthorizationCode, validatedRequest.Client.AccessTokenLifetime,
                cancellationToken);

        return new()
        {
            IdToken = idToken,
            AccessToken = accessToken,
            TokenType = Constants.AccessTokenTypes.DPoP,
            ExpiresIn = validatedRequest.Client.AccessTokenLifetime.Seconds,
            RefreshToken = refreshToken?.Token,
            AuthorizationDetails = validatedRequest.AuthorizationCode.AuthorizationDetails,
            Claims = validatedRequest.AuthorizationCode.Claims,
            GrantId = validatedRequest.AuthorizationCode.Grant?.GrantId
        };
    }

    private async Task<TokenResponse> GenerateForClientCredentialsGrant(ValidatedTokenRequest validatedRequest, string responseIssuer, CancellationToken cancellationToken)
    {
        var authorizationDetails = AuthorizationDetailExtensions.ReadFromJson(validatedRequest.RawRequest.AuthorizationDetails, out _);

        var utcNow = DateTime.UtcNow;
        var accessToken = await _accessTokenService.GenerateAsync(new AccessTokenPayload
        {
            Issuer = responseIssuer,
            Subject = validatedRequest.Client.ClientId,
            NotBefore = utcNow,
            Expiration = utcNow.AddSeconds(validatedRequest.Client.AccessTokenLifetime.Seconds),
            ClientId = validatedRequest.Client.ClientId,
            AuthorizationDetails = authorizationDetails,
            Jti = Guid.NewGuid(),
            Cnf = new CnfClaim
            {
                Pkh = Base64UrlEncoder.Encode(SelectPaserkHash(validatedRequest))
            }

        }, cancellationToken);

        return new()
        {
            AccessToken = accessToken,
            TokenType = Constants.AccessTokenTypes.DPoP,
            ExpiresIn = validatedRequest.Client.AccessTokenLifetime.Seconds,
            AuthorizationDetails = authorizationDetails
        };
    }

    private async Task<TokenResponse> GenerateForRefreshTokenGrant(ValidatedTokenRequest validatedRequest, string responseIssuer, CancellationToken cancellationToken)
    {
        if (validatedRequest.RefreshToken is null)
            throw new ArgumentException($"The {nameof(validatedRequest.RefreshToken)} cannot be null here", nameof(validatedRequest));

        var requestedTypes = validatedRequest.RawRequest.AuthorizationDetailsTypes?.FromSpaceDelimitedString() ?? Array.Empty<string>();

        var authorizationDetails = validatedRequest.RefreshToken.AuthorizationDetails.IntersectBy(requestedTypes, by => by.Type);
        var claims = validatedRequest.RawRequest.Claims.IsNullOrEmpty() ? validatedRequest.RefreshToken.Claims : validatedRequest.RawRequest.Claims.FromSpaceDelimitedString();

        var utcNow = DateTime.UtcNow;
        var accessToken = await _accessTokenService.GenerateAsync(new AccessTokenPayload
        {
            Issuer = responseIssuer,
            Subject = validatedRequest.RefreshToken.Subject,
            NotBefore = utcNow,
            Expiration = utcNow.AddSeconds(validatedRequest.Client.AccessTokenLifetime.Seconds),
            ClientId = validatedRequest.RefreshToken.ClientId,
            AuthorizationDetails = authorizationDetails,
            Claims = claims.ToSpaceDelimitedString(),
            Jti = Guid.NewGuid(),
            Cnf = new CnfClaim
            {
                Pkh = Base64UrlEncoder.Encode(SelectPaserkHash(validatedRequest))
            },
        }, cancellationToken);

        var newRefreshToken = await _refreshTokenService.RotateAsync(validatedRequest.RefreshToken, validatedRequest.Client, cancellationToken);

        return new()
        {
            AccessToken = accessToken,
            TokenType = Constants.AccessTokenTypes.DPoP,
            ExpiresIn = validatedRequest.Client.AccessTokenLifetime.Seconds,
            RefreshToken = newRefreshToken.Token,
            AuthorizationDetails = authorizationDetails,
            Claims = claims,
            GrantId = validatedRequest.RefreshToken.Grant.GrantId
        };
    }

    private async Task<TokenResponse> GenerateForCibaGrant(ValidatedTokenRequest validatedRequest, string responseIssuer, CancellationToken cancellationToken)
    {
        if (validatedRequest.CibaObject is null)
            throw new ArgumentException($"The {nameof(validatedRequest.CibaObject)} cannot be null here", nameof(validatedRequest));

        if (validatedRequest.CibaObject.Grant is null)
            throw new ArgumentException($"The {nameof(validatedRequest.CibaObject.Grant)} cannot be null here if access was not denied", nameof(validatedRequest));

        await _cibaObjectStore.RemoveAsync(validatedRequest.CibaObject.Id, cancellationToken);

        var utcNow = DateTime.UtcNow;
        var accessToken = await _accessTokenService.GenerateAsync(new AccessTokenPayload
        {
            Issuer = responseIssuer,
            Subject = validatedRequest.CibaObject.Subject,
            NotBefore = utcNow,
            Expiration = utcNow.AddSeconds(validatedRequest.Client.AccessTokenLifetime.Seconds),
            Jti = Guid.NewGuid(),
            ClientId = validatedRequest.Client.ClientId,
            AuthorizationDetails = validatedRequest.CibaObject.Grant.AuthorizationDetails,
            Claims = validatedRequest.CibaObject.Grant.Claims.ToSpaceDelimitedString(),
            Cnf = new CnfClaim
            {
                Pkh = Base64UrlEncoder.Encode(SelectPaserkHash(validatedRequest))
            }
        }, cancellationToken);

        var idToken = await _idTokenService.GenerateAsync(responseIssuer, validatedRequest.CibaObject, validatedRequest.Client.IdTokenLifetime,
            cancellationToken);

        RefreshToken? refreshToken = null;
        if (ShallIssueRefreshToken(validatedRequest.CibaObject.Grant.AuthorizationDetails))
            refreshToken = await _refreshTokenService.GenerateAsync(validatedRequest.CibaObject, validatedRequest.Client.AccessTokenLifetime,
                cancellationToken);

        return new()
        {
            IdToken = idToken,
            AccessToken = accessToken,
            TokenType = Constants.AccessTokenTypes.DPoP,
            ExpiresIn = validatedRequest.Client.AccessTokenLifetime.Seconds,
            RefreshToken = refreshToken?.Token,
            AuthorizationDetails = validatedRequest.CibaObject.Grant.AuthorizationDetails,
            Claims = validatedRequest.CibaObject.Grant.Claims,
            GrantId = validatedRequest.CibaObject.Grant.GrantId
        };
    }

    private static bool ShallIssueRefreshToken(IEnumerable<AuthorizationDetail> authorizationDetails)
    {
        return authorizationDetails.Any(p => p.Type == Constants.BuiltInAuthorizationDetails.OpenId.Type
            && p.Actions.ContainsKey(Constants.BuiltInAuthorizationDetails.OpenId.Actions.OfflineAccess));
    }

    private static byte[] SelectPaserkHash(ValidatedTokenRequest validatedRequest)
    {
        if (validatedRequest.AuthorizationCode?.DPoPPkh is not null)
            return validatedRequest.AuthorizationCode.DPoPPkh.Decode();
        else if (validatedRequest.CibaObject?.DPoPPkh is not null)
            return validatedRequest.CibaObject.DPoPPkh.Decode();
        else if (!validatedRequest.RawRequest.DPoP.IsNullOrEmpty())
            return HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(new PasetoToken(validatedRequest.RawRequest.DPoP).Footer));
        else
        {
            var paserk = Paserk.Encode(
                new PasetoAsymmetricPublicKey(validatedRequest.Client.Ed25519PublicKey.Value, new Version4()),
                PaserkType.Public);

            return HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(paserk));
        }
    }
}
