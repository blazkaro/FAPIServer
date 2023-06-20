using Json.Schema;

namespace FAPIServer.Storage.Models;

public class AuthorizationDetailSchema
{
    public string Type { get; set; }
    public bool Enabled { get; set; }
    public IEnumerable<AuthorizationDetailSchemaAction> SupportedActions { get; set; } = Array.Empty<AuthorizationDetailSchemaAction>();
    public IEnumerable<Uri> SupportedLocations { get; set; } = Array.Empty<Uri>();
    public JsonSchema? DefaultActionSchema { get; set; }
    public JsonSchema? ExtensionsSchema { get; set; }
    public bool IsReusable { get; set; }
    public bool ShowInDiscoveryDocument { get; set; } = true;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
