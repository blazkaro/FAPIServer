using FAPIServer.RequestHandling.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FAPIServer.Web.Controllers.Results;

public class ConsentPageActionResult : IActionResult
{
    private readonly AuthorizationRequest _authorizationRequest;

    public ConsentPageActionResult(AuthorizationRequest authorizationRequest)
    {
        _authorizationRequest = authorizationRequest;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var optionsMonitor = context.HttpContext.RequestServices.GetService<IOptionsMonitor<FapiWebOptions>>();
        var options = optionsMonitor?.CurrentValue ?? new FapiWebOptions();

        var returnUrl = Uri.EscapeDataString($"/fapi/authorization" +
            $"?client_id={_authorizationRequest.ClientId}" +
            $"&request_uri={_authorizationRequest.RequestUri}");

        var redirectUrl = $"{options.ConsentPath}?{options.ReturnUrlParamName}={returnUrl}";
        await new LocalRedirectResult(redirectUrl).ExecuteResultAsync(context);
    }
}
