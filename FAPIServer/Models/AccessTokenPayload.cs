using FAPIServer.Storage.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FAPIServer.Models;

public class AccessTokenPayload
{
    public AccessTokenPayload()
    {

    }

    public AccessTokenPayload(IDictionary<string, object> payload)
    {
        var atPayload = JsonSerializer.Deserialize<AccessTokenPayload>(JsonSerializer.Serialize(payload));
        if (atPayload is null)
            throw new JsonException("The payload could not be properly deserialized");

        Issuer = atPayload.Issuer;
        Subject = atPayload.Subject;
        NotBefore = atPayload.NotBefore;
        Expiration = atPayload.Expiration;
        Jti = atPayload.Jti;
        ClientId = atPayload.ClientId;
        Cnf = atPayload.Cnf;
        AuthorizationDetails = atPayload.AuthorizationDetails;
        Claims = atPayload.Claims;
    }

    [JsonPropertyName("iss")]
    public string Issuer { get; set; }

    [JsonPropertyName("sub")]
    public string Subject { get; set; }

    [JsonPropertyName("nbf")]
    public DateTime NotBefore { get; set; }

    [JsonPropertyName("exp")]
    public DateTime Expiration { get; set; }

    [JsonPropertyName("jti")]
    public Guid Jti { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    [JsonPropertyName("cnf")]
    public CnfClaim Cnf { get; set; }

    [JsonPropertyName("authorization_details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<AuthorizationDetail>? AuthorizationDetails { get; set; }

    [JsonPropertyName("claims")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Claims { get; set; }
}
