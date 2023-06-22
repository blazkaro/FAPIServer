using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Web.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Controllers.Results;

public class GrantQueryingActionResult : FapiWebSecureActionResult, IActionResult
{
    private readonly ResultDto _result;

    public GrantQueryingActionResult(GrantQueryingResponse response)
    {
        _result = new ResultDto { AuthorizationDetails = response.AuthorizationDetails, Claims = response.Claims };
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(_result, context.HttpContext.RequestAborted);
    }

    public override object GetResult() => _result;

    private sealed class ResultDto
    {
        [JsonPropertyName("authorization_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<AuthorizationDetail>? AuthorizationDetails { get; set; }

        [JsonPropertyName("claims")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Claims { get; set; }
    }
}
