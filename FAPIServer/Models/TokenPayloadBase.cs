using System.Text.Json;

namespace FAPIServer.Models;

public abstract class TokenPayloadBase<TPayload>
{
    public static TPayload? FromJson(string json)
    {
        return JsonSerializer.Deserialize<TPayload>(json);
    }
}
