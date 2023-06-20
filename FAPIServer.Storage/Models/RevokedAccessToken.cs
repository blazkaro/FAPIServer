namespace FAPIServer.Storage.Models;

public class RevokedAccessToken
{
    public string Jti { get; set; }
    public DateTime TokenExpiresAt { get; set; }
}
