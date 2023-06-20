namespace FAPIServer.ResponseHandling.Models;

public class PushedAuthorizationResponse
{
    public string RequestUri { get; set; }
    public int ExpiresIn { get; set; }
}
