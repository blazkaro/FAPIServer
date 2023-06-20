using System.Text.Json.Serialization;

namespace FAPIServer.Models;

public class CnfClaim
{
    [JsonPropertyName("pkh")]
    public string Pkh { get; set; }
}
