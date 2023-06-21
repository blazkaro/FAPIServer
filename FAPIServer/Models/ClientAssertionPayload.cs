using System.Text.Json;
using System.Text.Json.Serialization;

namespace FAPIServer.Models;

public class ClientAssertionPayload
{
    public static ClientAssertionPayload? FromJson(string json)
    {
        return JsonSerializer.Deserialize<ClientAssertionPayload>(json);
    }

    [JsonPropertyName("iss")]
    public string Issuer { get; set; }

    [JsonPropertyName("aud")]
    public string Audience { get; set; }

    [JsonPropertyName("sub")]
    public string Subject { get; set; }

    [JsonPropertyName("nbf")]
    public DateTime NotBefore { get; set; }

    [JsonPropertyName("exp")]
    public DateTime Expiration { get; set; }

    [JsonPropertyName("jti")]
    public Guid Jti { get; set; }
}
