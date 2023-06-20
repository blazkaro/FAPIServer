using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Controllers.Results;

public class GrantQueryingResult : IActionResult
{
    private readonly GrantQueryingResponse _response;

    public GrantQueryingResult(GrantQueryingResponse response)
    {
        _response = response;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var dto = new ResultDto { AuthorizationDetails = _response.AuthorizationDetails, Claims = _response.Claims };

        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(dto, context.HttpContext.RequestAborted);
    }

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
