using FAPIServer.Models;
using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Attributes;
using FAPIServer.Web.Authentication.PasetoDpop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/grants/{grantId}")]
[Authorize(AuthenticationSchemes = PasetoDpopAuthDefaults.AuthenticationScheme)]
[RequiredAuthorizationDetailAction(Constants.BuiltInAuthorizationDetails.OpenId.Type,
    Constants.BuiltInAuthorizationDetails.OpenId.Actions.GrantManagementRevoke)]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class GrantRevocationEndpoint : Endpoint<string>
{
    private readonly IGrantRevocationHandler _handler;

    public GrantRevocationEndpoint(IGrantRevocationHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete]
    public override async Task<IActionResult> HandleAsync(string grantId, CancellationToken cancellationToken)
    {
        var atPayload = JsonSerializer.Deserialize<AccessTokenPayload>(User.Claims.SingleOrDefault(p => p.Type == "at_payload")!.Value)!;
        var context = new GrantManagementContext(atPayload, grantId);

        var revoked = await _handler.HandleAsync(context, cancellationToken);
        if (!revoked)
            return NotFound();

        return NoContent();
    }
}
