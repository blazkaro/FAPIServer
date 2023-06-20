using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Controllers.Requests;
using FAPIServer.Web.Controllers.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/token/introspection")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class TokenIntrospectionEndpoint : Endpoint<TokenIntrospectionHttpRequest>
    .WithClientAuthentication
{
    private readonly ITokenIntrospectionHandler _handler;

    public TokenIntrospectionEndpoint(ITokenIntrospectionHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public override async Task<IActionResult> HandleAsync(ClientAuthHttpRequest authRequest, TokenIntrospectionHttpRequest request, CancellationToken cancellationToken)
    {
        var context = new TokenIntrospectionContext(authRequest, request,
            Request.GetRequestedEndpointUri(), Request.GetSchemeAndHost().ToString());

        var result = await _handler.HandleAsync(context, cancellationToken);
        if (!result.Success)
            return new ErrorActionResult(result.Error!.Value, result.FailureMessage);

        return new TokenIntrospectionResult(result.Response);
    }
}
