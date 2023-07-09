using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Contexts;
using FAPIServer.Web.Controllers.Requests;
using FAPIServer.Web.Controllers.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/authorization")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class AuthorizationEndpoint : Endpoint<AuthorizationHttpRequest>
{
    private readonly IAuthorizationHandler _handler;

    public AuthorizationEndpoint(IAuthorizationHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public override async Task<IActionResult> HandleAsync(AuthorizationHttpRequest request, CancellationToken cancellationToken)
    {
        var context = new AuthorizationContext(request, Request.GetSchemeAndHost().ToString(), User);
        var result = await _handler.HandleAsync(context, cancellationToken);
        if (!result.Success)
            return new ErrorActionResult(result.Error!.Value, result.FailureMessage);

        if (result.AuthenticationRequired)
            return Challenge();

        if (result.ConsentRequired)
            return new ConsentPageActionResult(request);

        return new AuthorizationActionResult(result.Response);
    }
}
