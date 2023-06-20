using FAPIServer.Storage.ValueObjects;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FAPIServer.Extensions;

public static class AuthorizationDetailExtensions
{
    public static bool IsIncludedIn(this IEnumerable<AuthorizationDetail> first, IEnumerable<AuthorizationDetail> second)
    {
        foreach (var firstAuthorizationDetail in first)
        {
            var secondAuthorizationDetail = second.SingleOrDefault(p => p.Type == firstAuthorizationDetail.Type);
            if (secondAuthorizationDetail == null)
                return false;

            if (!firstAuthorizationDetail.IsIncludedIn(secondAuthorizationDetail))
                return false;
        }

        return true;
    }

    public static IEnumerable<AuthorizationDetail> ReadFromJson(string? json, out int mismatchesCount)
    {
        mismatchesCount = 0;
        if (string.IsNullOrEmpty(json))
            return Array.Empty<AuthorizationDetail>();

        var node = JsonNode.Parse(json);

        List<JsonObject> objects = new();
        switch (node)
        {
            case JsonObject jObject:
                objects.Add(jObject);
                break;

            case JsonArray jArray:
                objects.AddRange(jArray.OfType<JsonObject>());
                mismatchesCount = jArray.Count - objects.Count;
                break;

            default:
                throw new JsonException("The JSON must be object or array");
        }

        List<AuthorizationDetail> result = new();
        foreach (var obj in objects)
        {
            var jsonObject = obj.ToString();
            if (AuthorizationDetail.CanCreate(jsonObject, out AuthorizationDetail? authorizationDetail))
                result.Add(authorizationDetail!);
            else
                mismatchesCount++;
        }

        return result;
    }
}
