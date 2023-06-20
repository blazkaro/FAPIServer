using Base64Url;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Serializers;
using FAPIServer.Services;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Models;
using Microsoft.Extensions.Options;
using Paseto;
using Paseto.Builder;
using System.Security.Cryptography;

namespace FAPIServer.ResponseHandling.Default;

public class AuthorizationResponseGenerator : IAuthorizationResponseGenerator
{
    private readonly FapiOptions _options;
    private readonly ISecretCredentialsStore _secretCredentialsStore;
    private readonly IAuthorizationRequestPersistenceService _authorizationRequestPersistenceService;
    private readonly IAuthorizationCodeStore _authorizationCodeStore;

    public AuthorizationResponseGenerator(IOptionsMonitor<FapiOptions> options,
        ISecretCredentialsStore secretCredentialsStore,
        IAuthorizationCodeStore authorizationCodeStore,
        IAuthorizationRequestPersistenceService authorizationRequestPersistenceService)
    {
        _options = options.CurrentValue;
        _secretCredentialsStore = secretCredentialsStore;
        _authorizationCodeStore = authorizationCodeStore;
        _authorizationRequestPersistenceService = authorizationRequestPersistenceService;
    }

    private async Task<PasetoBuilder> GetBasis(string issuer, string audience, CancellationToken cancellationToken)
    {
        var ed25519KeyPair = await _secretCredentialsStore.GetSigningCredentials(true, cancellationToken);
        var utcNow = DateTime.UtcNow;

        var responseObjectBasis = new PasetoBuilder()
            .WithJsonSerializer(new PasetoPayloadSerializer())
            .UseV4(Purpose.Public)
            .WithSecretKey(ed25519KeyPair.PrivateKey.Value.Concat(ed25519KeyPair.PublicKey.Value).ToArray())
            .Issuer(issuer)
            .Audience(audience)
            .NotBefore(utcNow)
            .Expiration(utcNow.AddSeconds(_options.AuthorizationResponseLifetime.Seconds));

        return responseObjectBasis;
    }

    public async Task<AuthorizationResponse> GenerateAsync(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest, Grant? similarGrant = null,
        CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (validatedRequest is null)
            throw new ArgumentNullException(nameof(validatedRequest));

        await _authorizationRequestPersistenceService.RemoveAsync(validatedRequest.ParObject, cancellationToken);

        List<AuthorizationDetail> authorizationDetails = new();
        IEnumerable<string> claims;
        if (validatedRequest.ParObject.RequestedGrant is not null || validatedRequest.ParObject.FreshGrant is not null)
        {
            var grant = (validatedRequest.ParObject.RequestedGrant ?? validatedRequest.ParObject.FreshGrant)!;
            // This is scenario where action was specified or fresh grant was created
            authorizationDetails.AddRange(grant.AuthorizationDetails);
            claims = grant.Claims;
        }
        else if (similarGrant is not null)
        {
            // This is scenario where consent was needed and similar grant was found from existing grants.
            foreach (var authorizationDetail in validatedRequest.ParObject.AuthorizationDetails)
            {
                var schema = validatedRequest.AuthorizationDetailSchemas.Single(p => p.Type == authorizationDetail.Type);
                var grantedAuthorizationDetail = similarGrant.AuthorizationDetails.Single(p => p.Type == authorizationDetail.Type);

                var result = authorizationDetail;
                foreach (var action in authorizationDetail.Actions.Keys)
                {
                    if (schema.SupportedActions.Single(p => p.Name == action).IsEnriched)
                        result.Actions[action] = grantedAuthorizationDetail.Actions[action];
                }

                authorizationDetails.Add(result);
            }

            claims = similarGrant.Claims.Intersect(validatedRequest.ParObject.Claims);
        }
        else
        {
            // This is scenario where client doesn't have to have user consent and fresh consent is not required
            authorizationDetails.AddRange(validatedRequest.ParObject.AuthorizationDetails);
            claims = validatedRequest.ParObject.Claims;
        }

        var code = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        var authorizationCode = new AuthorizationCode
        {
            Code = code,
            Subject = context.GetValidUser().Subject,
            ClientId = validatedRequest.Client.ClientId,
            State = validatedRequest.ParObject.State,
            Nonce = validatedRequest.ParObject.Nonce,
            AuthorizationDetails = authorizationDetails,
            Claims = claims,
            RedirectUri = validatedRequest.ParObject.RedirectUri,
            CodeChallenge = validatedRequest.ParObject.CodeChallenge,
            AuthTime = context.GetValidUser().AuthTime,
            Grant = validatedRequest.ParObject.RequestedGrant ?? validatedRequest.ParObject.FreshGrant ?? similarGrant,
            DPoPPkh = validatedRequest.ParObject.DPoPPkh,
            ExpiresAt = DateTime.UtcNow.AddSeconds(validatedRequest.Client.AuthorizationCodeLifetime.Seconds)
        };

        await _authorizationCodeStore.StoreAsync(authorizationCode, cancellationToken);

        var basis = await GetBasis(context.ResponseIssuer, validatedRequest.Client.ClientId, cancellationToken);
        var responseObject = basis
            .AddClaim("code", authorizationCode.Code)
            .AddClaim("state", authorizationCode.State)
            .Encode();

        return new()
        {
            ResponseObject = responseObject,
            RedirectUri = validatedRequest.ParObject.RedirectUri
        };
    }

    public async Task<AuthorizationResponse> GenerateAsync(AuthorizationContext context, ValidatedAuthorizationRequest validatedRequest, InteractionError interactionError,
        CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (validatedRequest is null)
            throw new ArgumentNullException(nameof(validatedRequest));

        await _authorizationRequestPersistenceService.RemoveAsync(validatedRequest.ParObject, cancellationToken);

        var basis = await GetBasis(context.ResponseIssuer, validatedRequest.Client.ClientId, cancellationToken);
        var responseObject = basis
          .AddClaim("error", interactionError.ToSnakeCase())
          .AddClaim("state", validatedRequest.ParObject.State)
          .Encode();

        return new()
        {
            ResponseObject = responseObject,
            RedirectUri = validatedRequest.ParObject.RedirectUri
        };
    }
}
