using FAPIServer.Models;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Controllers.Results;

public class TokenIntrospectionResult : IActionResult
{
    private readonly TokenIntrospectionResponse _response;

    public TokenIntrospectionResult(TokenIntrospectionResponse response)
    {
        _response = response;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var dto = new ResultDto
        {
            Active = _response.Active,
            ClientId = _response.ClientId,
            TokenType = _response.TokenType,
            NotBefore = _response.NotBefore,
            Expiration = _response.Expiration,
            Sub = _response.Sub,
            Cnf = _response.Cnf,
            AuthorizationDetails = _response.AuthorizationDetails,
            Jti = _response.Jti
        };

        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(dto, context.HttpContext.RequestAborted);
    }

    private sealed class ResultDto
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("client_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ClientId { get; set; }

        [JsonPropertyName("token_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TokenType { get; set; }

        [JsonPropertyName("nbf")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? NotBefore { get; set; }

        [JsonPropertyName("exp")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? Expiration { get; set; }

        [JsonPropertyName("sub")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Sub { get; set; }

        [JsonPropertyName("cnf")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CnfClaim? Cnf { get; set; }

        [JsonPropertyName("authorization_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<AuthorizationDetail>? AuthorizationDetails { get; set; }

        [JsonPropertyName("jti")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Jti { get; set; }
    }
}
