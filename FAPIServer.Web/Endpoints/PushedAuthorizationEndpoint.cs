using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Controllers.Requests;
using FAPIServer.Web.Controllers.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/par")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class PushedAuthorizationEndpoint : Endpoint<PushedAuthorizationHttpRequest>
    .WithClientAuthentication
{
    private readonly IPushedAuthorizationHandler _handler;

    public PushedAuthorizationEndpoint(IPushedAuthorizationHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public override async Task<IActionResult> HandleAsync(ClientAuthHttpRequest authRequest, PushedAuthorizationHttpRequest request, CancellationToken cancellationToken)
    {
        var context = new PushedAuthorizationContext(authRequest, request, Request.GetRequestedEndpointUri());
        var result = await _handler.HandleAsync(context, cancellationToken);
        if (!result.Success)
            return new ErrorActionResult(result.Error!.Value, result.FailureMessage);

        return new PushedAuthorizationActionResult(result.Response);
    }
}
