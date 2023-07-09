using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Web.Endpoints.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FAPIServer.Web.Endpoints;

[Route(".well-known/fapi-configuration")]
[ResponseCache(Duration = 1800, Location = ResponseCacheLocation.Any, NoStore = false)]
public class DiscoveryEndpoint : Endpoint
{
    private readonly IMemoryCache _memoryCache;
    private readonly FapiOptions _options;
    private readonly IAuthorizationDetailSchemaStore _authorizationDetailSchemaStore;
    private readonly IClaimsStore _claimsStore;

    public DiscoveryEndpoint(IMemoryCache memoryCache,
        IOptionsMonitor<FapiOptions> options,
        IAuthorizationDetailSchemaStore authorizationDetailSchemaStore,
        IClaimsStore claimsStore)
    {
        _memoryCache = memoryCache;
        _options = options.CurrentValue;
        _authorizationDetailSchemaStore = authorizationDetailSchemaStore;
        _claimsStore = claimsStore;
    }

    [HttpGet]
    public override async Task<IActionResult> HandleAsync(CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue("discovery", out DiscoveryActionResult? cachedResult) || cachedResult is null)
        {
            var result = new DiscoveryActionResult(await GenerateResponse(cancellationToken));
            lock (_memoryCache)
            {
                _memoryCache.Set("discovery", result, DateTime.UtcNow.AddSeconds(1800));
            }

            return result;
        }

        return cachedResult;
    }

    private async Task<DiscoveryResponse> GenerateResponse(CancellationToken cancellationToken)
    {
        var issuer = Request.GetSchemeAndHost().ToString();
        return new DiscoveryResponse
        {
            Issuer = issuer,
            PushedAuthorizationRequestEndpoint = $"{issuer}fapi/par",
            AuthorizationEndpoint = $"{issuer}fapi/authorization",
            TokenEndpoint = $"{issuer}fapi/token",
            TokenRevocationEndpoint = $"{issuer}fapi/token/revocation",
            TokenIntrospectionEndpoint = $"{issuer}fapi/token/introspection",
            GrantManagementEndpoint = $"{issuer}fapi/grants/",
            BackchannelAuthenticationEndpoint = $"{issuer}fapi/ciba",
            RequirePushedAuthorizationRequests = true,
            GrantManagementActionRequired = _options.GrantManagementActionRequired,
            PaserksUri = $"{issuer}fapi/paserks",
            AuthorizationDetailsTypesSupported = await _authorizationDetailSchemaStore.FindDiscoverableAsync(cancellationToken),
            ClaimsSupported = await _claimsStore.FindDiscoverableAsync(cancellationToken),
            GrantManagementActionsSupported = Constants.GrantManagementActions.Actions,
            ResponseTypesSupported = new string[] { "code" },
            GrantTypesSupported = Constants.GrantTypes.Types,
            AuthMethodsSupported = new string[] { Constants.ClientAuthenticationMethods.PrivateKeyPaseto },
            CodeChallengeMethodsSupported = new string[] { Constants.CodeChallengeMethods.S256 },
            BackchannelTokenDeliveryModesSupported = new string[] { Constants.CibaModes.Ping, Constants.CibaModes.Poll },
            BackchannelUserCodeParameterSupported = true
        };
    }
}
