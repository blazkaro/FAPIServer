using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Controllers.Requests;
using FAPIServer.Web.Controllers.Results;
using FAPIServer.Web.Endpoints.Requests;
using FAPIServer.Web.Endpoints.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/ciba")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class CibaEndpoint : Endpoint<CibaHttpRequest>
    .WithClientAuthentication
{
    private readonly ICibaHandler _handler;

    public CibaEndpoint(ICibaHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public override async Task<IActionResult> HandleAsync(ClientAuthHttpRequest authRequest, CibaHttpRequest request, CancellationToken cancellationToken)
    {
        var context = new CibaContext(authRequest, request, Request.GetRequestedEndpointUri(), Request.GetSchemeAndHost().ToString());
        var result = await _handler.HandleAsync(context, cancellationToken);

        if (!result.Success)
            return new ErrorActionResult(result.Error!.Value, result.FailureMessage);

        return new CibaActionResult(result.CibaResponse);
    }
}
