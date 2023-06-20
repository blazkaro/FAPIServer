namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;

public class UserClaim
{
    public int Id { get; set; }
    public string Type { get; set; }
    public bool Enabled { get; set; }
    public string Description { get; set; }
    public bool ShowInDiscoveryDocument { get; set; }
    public ICollection<Client> Clients { get; set; }
}
