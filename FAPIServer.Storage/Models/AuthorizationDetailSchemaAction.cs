using Json.Schema;

namespace FAPIServer.Storage.Models;

public class AuthorizationDetailSchemaAction
{
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public JsonSchema? ActionSchema { get; set; }
    public bool UseDefaultSchema { get; set; } = false;
    public bool IsEnriched { get; set; } = false;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
