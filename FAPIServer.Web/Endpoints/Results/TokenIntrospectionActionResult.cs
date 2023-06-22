using FAPIServer.Models;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Web.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Controllers.Results;

public class TokenIntrospectionActionResult : FapiWebSecureActionResult, IActionResult
{
    private readonly ResultDto _result;

    public TokenIntrospectionActionResult(TokenIntrospectionResponse response)
    {
        _result = new ResultDto
        {
            Active = response.Active,
            ClientId = response.ClientId,
            TokenType = response.TokenType,
            NotBefore = response.NotBefore,
            Expiration = response.Expiration,
            Sub = response.Sub,
            Cnf = response.Cnf,
            AuthorizationDetails = response.AuthorizationDetails,
            Jti = response.Jti
        };
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(_result, context.HttpContext.RequestAborted);
    }

    public override object GetResult() => _result;

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
