using FAPIServer.ResponseHandling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Controllers.Results;

public class AuthorizationActionResult : IActionResult
{
    private readonly AuthorizationResponse _response;

    public AuthorizationActionResult(AuthorizationResponse response)
    {
        _response = response;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status303SeeOther;
        context.HttpContext.Response.Headers.Location = $"{_response.RedirectUri.TrimEnd('/')}?response={_response.ResponseObject}";

        return Task.CompletedTask;
    }
}
