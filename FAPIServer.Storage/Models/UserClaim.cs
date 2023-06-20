namespace FAPIServer.Storage.Models;

public class UserClaim
{
    public string Type { get; set; }
    public bool Enabled { get; set; }
    public string Description { get; set; }
    public bool ShowInDiscoveryDocument { get; set; } = true;
}
