using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FAPIServer.Storage.ValueObjects;

public partial record AuthorizationDetail
{
    private AuthorizationDetail() { }

    public AuthorizationDetail(string json)
    {
        if (!CanCreate(json, out AuthorizationDetail? authorizationDetail) || authorizationDetail is null)
            throw new JsonException("The provided JSON is not well-formed JSON object or has format incompatible with authorization detail");

        Type = authorizationDetail.Type;
        Actions = authorizationDetail.Actions;
        Locations = authorizationDetail.Locations;
        Extensions = authorizationDetail.Extensions;
    }

    [JsonConstructor]
    [Newtonsoft.Json.JsonConstructor]
    public AuthorizationDetail(string type, IDictionary<string, object> actions, IEnumerable<string> locations)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException($"'{nameof(type)}' cannot be null or empty.", nameof(type));

        if (actions is null)
            throw new ArgumentNullException(nameof(actions));

        if (locations is null)
            throw new ArgumentNullException(nameof(locations));

        Type = type;
        Actions = actions;
        Locations = locations;
    }

    public AuthorizationDetail(string type, IDictionary<string, object> actions, IEnumerable<string> locations, IDictionary<string, object?>? extensions)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException($"'{nameof(type)}' cannot be null or empty.", nameof(type));

        if (actions is null)
            throw new ArgumentNullException(nameof(actions));

        if (locations is null)
            throw new ArgumentNullException(nameof(locations));

        Type = type;
        Actions = actions;
        Locations = locations;
        Extensions = extensions;
    }

    [JsonPropertyName("type")]
    [Newtonsoft.Json.JsonProperty("type")]
    public string Type { get; private set; }

    [JsonPropertyName("actions")]
    [Newtonsoft.Json.JsonProperty("actions")]
    public IDictionary<string, object> Actions { get; private set; }

    [JsonPropertyName("locations")]
    [Newtonsoft.Json.JsonProperty("locations")]
    public IEnumerable<string> Locations { get; private set; }

    [JsonExtensionData]
    [Newtonsoft.Json.JsonExtensionData]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, object?>? Extensions { get; private set; }

    public static bool CanCreate(string json, out AuthorizationDetail? authorizationDetail)
    {
        authorizationDetail = null;

        if (string.IsNullOrEmpty(json))
            return false;

        try
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json), true, default);
            if (!JsonElement.TryParseValue(ref reader, out JsonElement? element) || element is null || element.Value.ValueKind != JsonValueKind.Object)
                return false;

            if ((!element.Value.TryGetProperty("type", out JsonElement _type) || _type.ValueKind != JsonValueKind.String)
                || (!element.Value.TryGetProperty("actions", out JsonElement _actions) || _actions.ValueKind != JsonValueKind.Object)
                || (!element.Value.TryGetProperty("locations", out JsonElement _locations) || _locations.ValueKind != JsonValueKind.Array))
                return false;

            if (!_actions.EnumerateObject().All(p => p.Value.ValueKind == JsonValueKind.Object)
                || !_locations.EnumerateArray().All(p => p.ValueKind == JsonValueKind.String))
                return false;

            authorizationDetail = new AuthorizationDetail(
                _type.ToString(),
                _actions.EnumerateObject().ToDictionary(k => k.Name, v => v.Value.Deserialize<object>()!),
                _locations.EnumerateArray().Select(p => p.ToString()),
                element.Value.EnumerateObject().ExceptBy(new string[] { "type", "actions", "locations" }, by => by.Name)
                    .ToDictionary(k => k.Name, k => k.Value.Deserialize<object>())
                );

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// <para>Whether first authorization detail is included in second authorization detail.</para>
    /// <para>In practical words, whether the first authorization detail gives less or equal access in comparison to second authorization detail</para>
    /// </summary>
    /// <param name="first">The first <see cref="AuthorizationDetail"/></param>
    /// <param name="second">The second <see cref="AuthorizationDetail"/></param>
    /// <returns><c>true</c> if <paramref name="first"/> is included in <paramref name="second"/>; otherwise, <c>false</c></returns>
    public bool IsIncludedIn(AuthorizationDetail second)
    {
        if (second.Type != Type)
            return false;

        var firstGroupedIdentifiers = GetGroupedIdentifiers(JObject.Parse(JsonSerializer.Serialize(this)).Descendants().OfType<JValue>());
        var secondGroupedIdentifiers = GetGroupedIdentifiers(JObject.Parse(JsonSerializer.Serialize(second)).Descendants().OfType<JValue>());

        foreach (var firstGrouping in firstGroupedIdentifiers)
        {
            // sg - second grouping
            // fgId - identifier from first grouping
            // sgId - identifier from second grouping
            // Work: whether there is any grouping from secondGroupedIdentifiers where all of first grouping (currently iterated) identifiers are contained in second grouping
            var appropriateGroupingExists = secondGroupedIdentifiers.Any(sg =>
                firstGrouping.Select(fgId => fgId).All(fgId => sg.Select(sgId => sgId).Contains(fgId)));

            if (!appropriateGroupingExists)
                return false;
        }

        return true;
    }

    public void Merge(AuthorizationDetail second)
    {
        if (second.Type != Type)
            return;

        var firstObject = JObject.Parse(JsonSerializer.Serialize(this));
        var secondObject = JObject.Parse(JsonSerializer.Serialize(second));

        firstObject.Merge(secondObject, new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union,
            PropertyNameComparison = StringComparison.OrdinalIgnoreCase
        });

        var merged = new AuthorizationDetail(firstObject.ToString());

        this.Actions = merged.Actions;
        this.Locations = merged.Locations;
        this.Extensions = merged.Extensions;
    }

    [GeneratedRegex(@"\[\d+\]", RegexOptions.Compiled)]
    private static partial Regex FindArrayIndexers();

    private static IEnumerable<IGrouping<string, string>> GetGroupedIdentifiers(IEnumerable<JValue> values)
    {
        // Group by path to last ] character, so to the last array indexer.
        // Doing this we avoid mixing properties from different objects inside one array but with the same properties with the same values
        return values.GroupBy(p => p.Path.IndexOf(']') != -1 ? p.Path[0..(p.Path.LastIndexOf(']') + 1)] : p.Path,
            v => $"{FindArrayIndexers().Replace(v.Path, string.Empty)}:{v.Value?.ToString()}");
    }
}
