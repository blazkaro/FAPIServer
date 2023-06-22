using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints.Results;

public class UserInfoActionResult : FapiWebSecureActionResult, IActionResult
{
    private readonly IDictionary<string, object> _claims;

    public UserInfoActionResult(IDictionary<string, object> claims)
    {
        _claims = claims;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(_claims, context.HttpContext.RequestAborted);
    }

    public override object GetResult() => _claims;
}
