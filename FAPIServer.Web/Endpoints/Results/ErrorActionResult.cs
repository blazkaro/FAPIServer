using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Controllers.Results;

public class ErrorActionResult : IActionResult
{
    private readonly Error _error;
    private readonly string? _errorDescription;

    public ErrorActionResult(Error error)
    {
        _error = error;
    }

    public ErrorActionResult(Error error, string? errorDescription) : this(error)
    {
        _errorDescription = errorDescription;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var info = ProcessError();
        var dto = new ResultDto { Error = info.SnakeCaseName, ErrorDescription = _errorDescription };

        context.HttpContext.Response.StatusCode = info.StatusCode;
        await context.HttpContext.Response.WriteAsJsonAsync(dto, context.HttpContext.RequestAborted);
    }

    private sealed class ResultDto
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorDescription { get; set; }
    }

    private (string SnakeCaseName, int StatusCode) ProcessError()
    {
        return _error switch
        {
            Error.InvalidClient => new("invalid_client", StatusCodes.Status401Unauthorized),
            Error.UnauthorizedClient => new("unauthorized_client", StatusCodes.Status403Forbidden),
            Error.InvalidRequest => new("invalid_request", StatusCodes.Status400BadRequest),
            Error.InvalidGrantId => new("invalid_grant_id", StatusCodes.Status400BadRequest),
            Error.InvalidClaims => new("invalid_claims", StatusCodes.Status400BadRequest),
            Error.InvalidRedirectUri => new("invalid_redirect_uri", StatusCodes.Status400BadRequest),
            Error.InvalidAuthorizationDetails => new("invalid_authorization_details", StatusCodes.Status400BadRequest),
            Error.InvalidAuthorizationDetaiTypes => new("invalid_authorization_detail_types", StatusCodes.Status400BadRequest),
            Error.InvalidRequestUri => new("invalid_request_uri", StatusCodes.Status400BadRequest),
            Error.UnsupportedGrantType => new("unsupported_grant_type", StatusCodes.Status400BadRequest),
            Error.InvalidGrant => new("invalid_grant", StatusCodes.Status400BadRequest),
            Error.InvalidDPoPProof => new("invalid_dpop_proof", StatusCodes.Status400BadRequest),
            _ => new("unexpected_error", StatusCodes.Status500InternalServerError)
        };
    }
}
