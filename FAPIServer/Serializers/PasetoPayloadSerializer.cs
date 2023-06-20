using Paseto.Serializers;
using System.Text.Json;

namespace FAPIServer.Serializers;

/// <summary>
/// <para>This class is needed to properly serialize objects using <see cref="Paseto.Builder.PasetoBuilder.Encode"/>.</para>
/// <para>It's because default implementation uses <see cref="Newtonsoft.Json"/> which badly serializes and deserializes <see cref="object"/></para>
/// <para>So we use this class to avoid problems with serializing e.g. authorization details</para>
/// </summary>
public class PasetoPayloadSerializer : IJsonSerializer
{
    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }

    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}
