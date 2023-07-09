using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Models;
using System.Security.Cryptography;

namespace FAPIServer.ResponseHandling.Default;

public class PushedAuthorizationResponseGenerator : IPushedAuthorizationResponseGenerator
{
    private readonly IParObjectStore _parObjectStore;

    public PushedAuthorizationResponseGenerator(IParObjectStore parObjectStore)
    {
        _parObjectStore = parObjectStore;
    }

    public async Task<PushedAuthorizationResponse> GenerateAsync(ValidatedPushedAuthorizationRequest validatedRequest, CancellationToken cancellationToken = default)
    {
        if (validatedRequest is null)
            throw new ArgumentNullException(nameof(validatedRequest));

        var uri = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        var parObject = new ParObject
        {
            Uri = uri,
            ClientId = validatedRequest.Client.ClientId,
            AuthorizationDetails = validatedRequest.AuthorizationDetails,
            Claims = validatedRequest.Claims.ToHashSet(),
            RedirectUri = validatedRequest.RawRequest.RedirectUri,
            State = validatedRequest.RawRequest.State,
            Nonce = validatedRequest.RawRequest.Nonce,
            CodeChallengeMethod = validatedRequest.RawRequest.CodeChallengeMethod,
            CodeChallenge = new Base64UrlEncodedString(validatedRequest.RawRequest.CodeChallenge),
            Grant = validatedRequest.RequestedGrant,
            GrantManagementAction = validatedRequest.RawRequest.GrantManagementAction,
            Prompt = validatedRequest.RawRequest.Prompt,
            MaxAge = validatedRequest.RawRequest.MaxAge,
            DPoPPkh = !validatedRequest.RawRequest.DPoPPkh.IsNullOrEmpty() ? new Base64UrlEncodedString(validatedRequest.RawRequest.DPoPPkh) : null,
            ExpiresAt = DateTime.UtcNow.AddSeconds(validatedRequest.Client.RequestUriLifetime.Seconds)
        };

        await _parObjectStore.StoreAsync(parObject, cancellationToken);

        return new()
        {
            RequestUri = parObject.Uri,
            ExpiresIn = validatedRequest.Client.RequestUriLifetime.Seconds
        };
    }
}
