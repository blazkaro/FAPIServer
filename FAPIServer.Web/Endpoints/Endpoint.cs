using FAPIServer.Web.Controllers.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints;

public abstract class Endpoint<TRequest> : ControllerBase
{
    public abstract class WithClientAuthentication : ControllerBase
    {
        public abstract Task<IActionResult> HandleAsync(ClientAuthHttpRequest authRequest, TRequest request, CancellationToken cancellationToken);

    }

    public abstract Task<IActionResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public abstract class Endpoint : ControllerBase
{
    public abstract Task<IActionResult> HandleAsync(CancellationToken cancellationToken);
}
