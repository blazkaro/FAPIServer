using FAPIServer.ResponseHandling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Controllers.Results;

public class PushedAuthorizationActionResult : IActionResult
{
    private readonly PushedAuthorizationResponse _response;

    public PushedAuthorizationActionResult(PushedAuthorizationResponse response)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        _response.RequestUri = _response.RequestUri.StartsWith(Constants.RequestUriUrn)
            ? _response.RequestUri
            : $"{Constants.RequestUriUrn}{_response.RequestUri}";

        var dto = new ResultDto { RequestUri = _response.RequestUri, ExpiresIn = _response.ExpiresIn };

        context.HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        context.HttpContext.Response.Headers.Location = dto.RequestUri;
        await context.HttpContext.Response.WriteAsJsonAsync(dto, context.HttpContext.RequestAborted);
    }

    private sealed class ResultDto
    {
        [JsonPropertyName("request_uri")]
        public string RequestUri { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
