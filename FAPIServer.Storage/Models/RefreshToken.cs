using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.Models;

public class RefreshToken
{
    public string Token { get; set; }
    public string ClientId { get; set; }
    public string Subject { get; set; }
    public IEnumerable<AuthorizationDetail> AuthorizationDetails { get; set; } = Array.Empty<AuthorizationDetail>();
    public IEnumerable<string> Claims { get; set; } = Array.Empty<string>();
    public Grant Grant { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool HasExpired() => DateTime.UtcNow >= ExpiresAt;
}
