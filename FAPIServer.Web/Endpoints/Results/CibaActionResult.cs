using FAPIServer.ResponseHandling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Endpoints.Results;

public class CibaActionResult : IActionResult
{
    private readonly CibaResponse _response;

    public CibaActionResult(CibaResponse response)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var dto = new ResultDto { AuthReqId = _response.AuthReqId, ExpiresIn = _response.ExpiresIn, Interval = _response.Interval };

        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(dto, context.HttpContext.RequestAborted);
    }

    private sealed class ResultDto
    {
        [JsonPropertyName("auth_req_id")]
        public string AuthReqId { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("interval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Interval { get; set; }
    }
}
