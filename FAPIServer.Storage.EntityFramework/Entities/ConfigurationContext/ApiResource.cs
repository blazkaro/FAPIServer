namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;

public class ApiResource
{
    public int Id { get; set; }
    public bool Enabled { get; set; }
    public Uri Uri { get; set; }
    public Client Client { get; set; }
    public int ClientId { get; set; }
    public ICollection<AuthorizationDetailSchema> AuthorizationDetailSchemas { get; set; }
}
