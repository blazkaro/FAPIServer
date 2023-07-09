namespace FAPIServer.ResponseHandling.Models;

public class DiscoveryResponse
{
    public string Issuer { get; set; }
    public string PushedAuthorizationRequestEndpoint { get; set; }
    public string AuthorizationEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string TokenRevocationEndpoint { get; set; }
    public string TokenIntrospectionEndpoint { get; set; }
    public string GrantManagementEndpoint { get; set; }
    public string BackchannelAuthenticationEndpoint { get; set; }
    public bool RequirePushedAuthorizationRequests { get; set; }
    public bool GrantManagementActionRequired { get; set; }
    public bool BackchannelUserCodeParameterSupported { get; set; }
    public string PaserksUri { get; set; }
    public IEnumerable<string> AuthorizationDetailsTypesSupported { get; set; } = Array.Empty<string>();
    public IEnumerable<string> ClaimsSupported { get; set; } = Array.Empty<string>();
    public IEnumerable<string> GrantManagementActionsSupported { get; set; } = Array.Empty<string>();
    public IEnumerable<string> ResponseTypesSupported { get; set; } = Array.Empty<string>();
    public IEnumerable<string> GrantTypesSupported { get; set; } = Array.Empty<string>();
    public IEnumerable<string> AuthMethodsSupported { get; set; } = Array.Empty<string>();
    public IEnumerable<string> CodeChallengeMethodsSupported { get; set; } = Array.Empty<string>();
    public IEnumerable<string> BackchannelTokenDeliveryModesSupported { get; set; } = Array.Empty<string>();
}
