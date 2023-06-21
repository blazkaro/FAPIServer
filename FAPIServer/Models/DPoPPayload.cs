using System.Text.Json;

namespace FAPIServer.Models;

public class DPoPPayload
{
    public static DPoPPayload? FromJson(string json)
    {
        return JsonSerializer.Deserialize<DPoPPayload>(json);
    }

    public DateTime NotBefore { get; set; }
    public DateTime Expiration { get; set; }
    public string Htm { get; set; }
    public string Htu { get; set; }
    public string Ath { get; set; }
    public Guid Jti { get; set; }
}
