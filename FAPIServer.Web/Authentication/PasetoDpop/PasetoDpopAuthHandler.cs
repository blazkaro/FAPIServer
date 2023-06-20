using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.Validation;
using FAPIServer.Validation.Contexts;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSec.Cryptography;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FAPIServer.Web.Authentication.PasetoDpop;

public class PasetoDpopAuthHandler : AuthenticationHandler<PasetoDpopAuthOptions>
{
    private readonly IDPoPProofValidator _dpopProofValidator;
    private readonly IAccessTokenValidator _accessTokenValidator;

    public PasetoDpopAuthHandler(IOptionsMonitor<PasetoDpopAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
        IDPoPProofValidator dpopProofValidator,
        IAccessTokenValidator accessTokenValidator)
        : base(options, logger, encoder, clock)
    {
        _dpopProofValidator = dpopProofValidator;
        _accessTokenValidator = accessTokenValidator;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();
        if (authorizationHeader.IsNullOrEmpty() || !authorizationHeader.StartsWith("DPoP"))
            return AuthenticateResult.NoResult();

        var accessToken = authorizationHeader.Replace("DPoP ", string.Empty);
        if (accessToken.IsNullOrEmpty())
            return Fail("The access token is not present");

        var dpop = Request.Headers["DPoP"].ToString();
        if (dpop.IsNullOrEmpty())
            return Fail("The DPoP proof is not present", true);

        var accessTokenValidationResult = await _accessTokenValidator.ValidateAsync(Request.GetSchemeAndHost().ToString(),
            accessToken, Context.RequestAborted);

        if (!accessTokenValidationResult.IsValid)
            return Fail(accessTokenValidationResult.FailureMessage);

        var pkh = (string)accessTokenValidationResult.Payload.SingleOrDefault(p => p.Key == "pkh").Value;
        var dpopProofValidationResult = _dpopProofValidator.Validate(dpop, new DPoPValidationParameters(
            Request.GetRequestedEndpointUri(),
            Request.Method,
            Base64UrlEncoder.Decode(pkh),
            HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes(accessToken))));

        if (!dpopProofValidationResult.IsValid)
            return Fail(dpopProofValidationResult.FailureMessage, true);

        var claims = new List<Claim>
        {
            new("at_payload", JsonSerializer.Serialize(accessTokenValidationResult.Payload))
        };

        var claimsIdentity = new ClaimsIdentity(claims, PasetoDpopAuthDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authTicket = new AuthenticationTicket(claimsPrincipal, PasetoDpopAuthDefaults.AuthenticationScheme);
        return AuthenticateResult.Success(authTicket);
    }

    private AuthenticateResult Fail(string? description, bool dpopFailed = false)
    {
        Context.Response.OnStarting(async () =>
        {
            string errorCode = dpopFailed ? "invalid_dpop_proof" : "invalid_token";
            await Context.Response.WriteAsJsonAsync(new { error = errorCode, error_description = description });
        });

        return AuthenticateResult.Fail(description);
    }
}
