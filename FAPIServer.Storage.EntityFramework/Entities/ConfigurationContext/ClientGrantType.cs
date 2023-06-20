namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;

public class ClientGrantType
{
    public int Id { get; set; }
    public string GrantType { get; set; }
    public Client Client { get; set; }
    public int ClientId { get; set; }
}
