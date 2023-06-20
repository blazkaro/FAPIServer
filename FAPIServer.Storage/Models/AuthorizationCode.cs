using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.Models;

public class AuthorizationCode
{
    public string Code { get; set; }
    public string Subject { get; set; }
    public string ClientId { get; set; }
    public string Nonce { get; set; }
    public string State { get; set; }
    public IEnumerable<AuthorizationDetail> AuthorizationDetails { get; set; } = Array.Empty<AuthorizationDetail>();
    public IEnumerable<string> Claims { get; set; } = Array.Empty<string>();
    public string RedirectUri { get; set; }
    public Base64UrlEncodedString CodeChallenge { get; set; }
    public DateTime AuthTime { get; set; }
    public Grant? Grant { get; set; }
    public Base64UrlEncodedString? DPoPPkh { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool HasExpired() => DateTime.UtcNow >= ExpiresAt;
}
