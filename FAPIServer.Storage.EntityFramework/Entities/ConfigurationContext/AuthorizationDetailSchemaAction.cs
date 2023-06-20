using Json.Schema;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;

public class AuthorizationDetailSchemaAction
{
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public AuthorizationDetailSchema AuthorizationDetailSchema { get; set; }
    public int AuthorizationDetailSchemaId { get; set; }
    public JsonSchema? ActionSchema { get; set; }
    public bool UseDefaultSchema { get; set; }
    public bool IsEnriched { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public ICollection<Client> Clients { get; set; }
}
