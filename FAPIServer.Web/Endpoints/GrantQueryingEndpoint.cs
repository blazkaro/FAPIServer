using FAPIServer.Models;
using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Attributes;
using FAPIServer.Web.Authentication.PasetoDpop;
using FAPIServer.Web.Controllers.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/grants/{grantId}")]
[Authorize(AuthenticationSchemes = PasetoDpopAuthDefaults.AuthenticationScheme)]
[RequiredAuthorizationDetailAction(Constants.BuiltInAuthorizationDetails.OpenId.Type,
    Constants.BuiltInAuthorizationDetails.OpenId.Actions.GrantManagementQuery)]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[ServiceFilter(typeof(SecureResponseAttribute), IsReusable = false)]
public class GrantQueryingEndpoint : Endpoint<string>
{
    private readonly IGrantQueryingHandler _handler;

    public GrantQueryingEndpoint(IGrantQueryingHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public override async Task<IActionResult> HandleAsync(string grantId, CancellationToken cancellationToken)
    {
        var atPayload = JsonSerializer.Deserialize<AccessTokenPayload>(User.Claims.SingleOrDefault(p => p.Type == "at_payload")!.Value)!;
        var context = new GrantManagementContext(atPayload, grantId);

        var result = await _handler.HandleAsync(context, cancellationToken);
        if (!result.Success)
            return NotFound();

        HttpContext.Items.Add(WebConstants.ResponseAudienceKey, atPayload.ClientId);
        return new GrantQueryingActionResult(result.Response);
    }
}
