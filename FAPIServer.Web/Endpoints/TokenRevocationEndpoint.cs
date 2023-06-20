using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Controllers.Requests;
using FAPIServer.Web.Controllers.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/token/revocation")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class TokenRevocationEndpoint : Endpoint<TokenRevocationHttpRequest>
    .WithClientAuthentication
{
    private readonly ITokenRevocationHandler _handler;

    public TokenRevocationEndpoint(ITokenRevocationHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public override async Task<IActionResult> HandleAsync(ClientAuthHttpRequest authRequest, TokenRevocationHttpRequest request, CancellationToken cancellationToken)
    {
        var context = new TokenRevocationContext(authRequest, request,
            Request.GetRequestedEndpointUri(), Request.GetSchemeAndHost().ToString());

        var result = await _handler.HandleAsync(context, cancellationToken);
        if (!result.Success)
            return new ErrorActionResult(result.Error!.Value, result.FailureMessage);

        return Ok();
    }
}
