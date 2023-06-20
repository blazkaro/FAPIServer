using Json.Schema;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class JsonSchemaConverter : ValueConverter<JsonSchema, string>
{
    private static string Serialize(JsonSchema jsonSchema) => JsonSerializer.Serialize(jsonSchema);
    private static JsonSchema FromJson(string jsonSchema) => JsonSchema.FromText(jsonSchema);

    public JsonSchemaConverter()
        : base(
            v => Serialize(v),
            v => FromJson(v))
    {

    }
}
