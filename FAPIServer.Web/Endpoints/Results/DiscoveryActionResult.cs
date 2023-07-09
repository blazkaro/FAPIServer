using FAPIServer.ResponseHandling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Endpoints.Results;

public class DiscoveryActionResult : IActionResult
{
    private readonly DiscoveryResponse _response;

    public DiscoveryActionResult(DiscoveryResponse response)
    {
        _response = response;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var dto = new ResultDto
        {
            Issuer = _response.Issuer,
            PushedAuthorizationRequestEndpoint = _response.PushedAuthorizationRequestEndpoint,
            AuthorizationEndpoint = _response.AuthorizationEndpoint,
            TokenEndpoint = _response.TokenEndpoint,
            TokenRevocationEndpoint = _response.TokenRevocationEndpoint,
            TokenIntrospectionEndpoint = _response.TokenIntrospectionEndpoint,
            GrantManagementEndpoint = _response.GrantManagementEndpoint,
            BackchannelAuthenticationEndpoint = _response.BackchannelAuthenticationEndpoint,
            RequirePushedAuthorizationRequests = _response.RequirePushedAuthorizationRequests,
            GrantManagementActionRequired = _response.GrantManagementActionRequired,
            PaserksUri = _response.PaserksUri,
            AuthorizationDetailsTypesSupported = _response.AuthorizationDetailsTypesSupported,
            ClaimsSupported = _response.ClaimsSupported,
            GrantManagementActionsSupported = _response.GrantManagementActionsSupported,
            ResponseTypesSupported = _response.ResponseTypesSupported,
            GrantTypesSupported = _response.GrantTypesSupported,
            AuthMethodsSupported = _response.AuthMethodsSupported,
            CodeChallengeMethodsSupported = _response.CodeChallengeMethodsSupported,
            BackchannelTokenDeliveryModesSupported = _response.BackchannelTokenDeliveryModesSupported,
            BackchannelUserCodeParameterSupported = _response.BackchannelUserCodeParameterSupported
        };

        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(dto, context.HttpContext.RequestAborted);
    }

    private sealed class ResultDto
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("pushed_authorization_request_endpoint")]
        public string PushedAuthorizationRequestEndpoint { get; set; }

        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("token_revocation_endpoint")]
        public string TokenRevocationEndpoint { get; set; }

        [JsonPropertyName("token_introspection_endpoint")]
        public string TokenIntrospectionEndpoint { get; set; }

        [JsonPropertyName("grant_management_endpoint")]
        public string GrantManagementEndpoint { get; set; }

        [JsonPropertyName("backchannel_authentication_endpoint")]
        public string BackchannelAuthenticationEndpoint { get; set; }

        [JsonPropertyName("require_pushed_authorization_requests")]
        public bool RequirePushedAuthorizationRequests { get; set; }

        [JsonPropertyName("grant_management_action_required")]
        public bool GrantManagementActionRequired { get; set; }

        [JsonPropertyName("backchannel_user_code_parameter_supported")]
        public bool BackchannelUserCodeParameterSupported { get; set; }

        [JsonPropertyName("paserks_uri")]
        public string PaserksUri { get; set; }

        [JsonPropertyName("authorization_details_types_supported")]
        public IEnumerable<string> AuthorizationDetailsTypesSupported { get; set; }

        [JsonPropertyName("claims_supported")]
        public IEnumerable<string> ClaimsSupported { get; set; }

        [JsonPropertyName("grant_management_actions_supported")]
        public IEnumerable<string> GrantManagementActionsSupported { get; set; }

        [JsonPropertyName("response_types_supported")]
        public IEnumerable<string> ResponseTypesSupported { get; set; }

        [JsonPropertyName("grant_types_supported")]
        public IEnumerable<string> GrantTypesSupported { get; set; }

        [JsonPropertyName("auth_methods_supported")]
        public IEnumerable<string> AuthMethodsSupported { get; set; }

        [JsonPropertyName("code_challenge_methods_supported")]
        public IEnumerable<string> CodeChallengeMethodsSupported { get; set; }

        [JsonPropertyName("backchannel_token_delivery_modes_supported")]
        public IEnumerable<string> BackchannelTokenDeliveryModesSupported { get; set; }
    }
}
