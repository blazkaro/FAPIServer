using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Controllers.Results;

public class TokenResult : IActionResult
{
    private readonly TokenResponse _response;

    public TokenResult(TokenResponse response)
    {
        _response = response;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var dto = new ResultDto
        {
            IdToken = _response.IdToken,
            AccessToken = _response.AccessToken,
            TokenType = _response.TokenType,
            ExpiresIn = _response.ExpiresIn,
            RefreshToken = _response.RefreshToken,
            AuthorizationDetails = _response.AuthorizationDetails,
            Claims = _response.Claims,
            GrantId = _response.GrantId
        };

        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(dto, context.HttpContext.RequestAborted);
    }

    private sealed class ResultDto
    {
        [JsonPropertyName("id_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IdToken { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("authorization_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<AuthorizationDetail>? AuthorizationDetails { get; set; }

        [JsonPropertyName("claims")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Claims { get; set; }

        [JsonPropertyName("grant_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? GrantId { get; set; }
    }
}
