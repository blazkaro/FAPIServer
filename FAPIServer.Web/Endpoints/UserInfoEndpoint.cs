using FAPIServer.Models;
using FAPIServer.RequestHandling;
using FAPIServer.Web.Attributes;
using FAPIServer.Web.Authentication.PasetoDpop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FAPIServer.Web.Endpoints;

[Route("oidc/userinfo")]
[Authorize(AuthenticationSchemes = PasetoDpopAuthDefaults.AuthenticationScheme)]
[RequiredAuthorizationDetailAction(Constants.BuiltInAuthorizationDetails.OpenId.Type)]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class UserInfoEndpoint : Endpoint
{
    private readonly IUserInfoHandler _handler;

    public UserInfoEndpoint(IUserInfoHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    [HttpPost]
    public override async Task<IActionResult> HandleAsync(CancellationToken cancellationToken)
    {
        var atPayload = JsonSerializer.Deserialize<AccessTokenPayload>(User.Claims.Single(p => p.Type == "at_payload").Value)!;
        var userClaims = await _handler.HandleAsync(atPayload, cancellationToken);

        return Ok(userClaims);
    }
}
