namespace FAPIServer.Storage.Models;

public class RevokedClientAssertion
{
    public string Jti { get; set; }
    public string ClientId { get; set; }
    public DateTime AssertionExpiresAt { get; set; }
}
