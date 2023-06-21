using System.Text.Json;

namespace FAPIServer.Models;

public class ClientAssertionPayload
{
    public static ClientAssertionPayload? FromJson(string json)
    {
        return JsonSerializer.Deserialize<ClientAssertionPayload>(json);
    }

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Subject { get; set; }
    public DateTime NotBefore { get; set; }
    public DateTime Expiration { get; set; }
    public Guid Jti { get; set; }
}
