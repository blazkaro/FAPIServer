using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.Models;

public class CibaObject
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public IEnumerable<AuthorizationDetail> AuthorizationDetails { get; set; } = Array.Empty<AuthorizationDetail>();
    public IEnumerable<string> Claims { get; set; } = Array.Empty<string>();
    public string? ClientNotificationToken { get; set; }
    public string Subject { get; set; }
    public string? BindingMessage { get; set; }
    public Grant? Grant { get; set; }
    //public Grant? RequestedGrant { get; set; }
    public string? GrantManagementAction { get; set; }
    public Base64UrlEncodedString? DPoPPkh { get; set; }
    public DateTime ExpiresAt { get; set; }
    //public Grant? FreshGrant { get; set; }
    public bool IsCompleted { get; set; }
    public bool AccessDenied { get; set; }

    public bool HasExpired() => DateTime.UtcNow >= ExpiresAt;
}
