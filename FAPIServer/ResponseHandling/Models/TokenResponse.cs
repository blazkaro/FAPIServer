using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.ResponseHandling.Models;

public class TokenResponse
{
    public string? IdToken { get; set; }
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
    public IEnumerable<AuthorizationDetail>? AuthorizationDetails { get; set; }
    public IEnumerable<string>? Claims { get; set; }
    public string? GrantId { get; set; }
}
