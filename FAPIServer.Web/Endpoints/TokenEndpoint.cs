using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Controllers.Requests;
using FAPIServer.Web.Controllers.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/token")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class TokenEndpoint : Endpoint<TokenHttpRequest>
    .WithClientAuthentication
{
    private readonly ITokenHandler _handler;

    public TokenEndpoint(ITokenHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public override async Task<IActionResult> HandleAsync(ClientAuthHttpRequest authRequest, TokenHttpRequest request, CancellationToken cancellationToken)
    {
        var context = new TokenContext(authRequest, request, Request.GetRequestedEndpointUri(),
            Request.Method, Request.GetSchemeAndHost().ToString());

        var result = await _handler.HandleAsync(context, cancellationToken);
        if (!result.Success)
            return new ErrorActionResult(result.Error!.Value, result.FailureMessage);

        return new TokenResult(result.TokenResponse);
    }
}
