using Json.Schema;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;

public class AuthorizationDetailSchema
{
    public int Id { get; set; }
    public string Type { get; set; }
    public bool Enabled { get; set; }
    public ICollection<AuthorizationDetailSchemaAction> AuthorizationDetailSchemaActions { get; set; }
    public ICollection<ApiResource> ApiResources { get; set; }
    public JsonSchema? DefaultActionSchema { get; set; }
    public JsonSchema? ExtensionsSchema { get; set; }
    public bool IsReusable { get; set; }
    public bool ShowInDiscoveryDocument { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
