namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;

public class ClientRedirectUri
{
    public int Id { get; set; }
    public Uri Uri { get; set; }
    public Client Client { get; set; }
    public int ClientId { get; set; }
}
