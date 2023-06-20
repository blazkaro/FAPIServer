namespace FAPIServer.Storage.Models;

public class ClientAllowedSchema
{
    public string SchemaType { get; set; }
    public IEnumerable<string> AllowedActions { get; set; }
}
