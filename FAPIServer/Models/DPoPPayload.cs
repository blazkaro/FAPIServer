using System.Text.Json;
using System.Text.Json.Serialization;

namespace FAPIServer.Models;

public class DPoPPayload
{
    public static DPoPPayload? FromJson(string json)
    {
        return JsonSerializer.Deserialize<DPoPPayload>(json);
    }

    [JsonPropertyName("nbf")]
    public DateTime NotBefore { get; set; }

    [JsonPropertyName("exp")]
    public DateTime Expiration { get; set; }

    [JsonPropertyName("htm")]
    public string Htm { get; set; }

    [JsonPropertyName("htu")]
    public string Htu { get; set; }

    [JsonPropertyName("ath")]
    public string Ath { get; set; }

    [JsonPropertyName("jti")]
    public Guid Jti { get; set; }
}
