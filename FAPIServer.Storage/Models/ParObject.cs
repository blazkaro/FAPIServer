using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.Models;

public class ParObject
{
    public string Uri { get; set; }
    public string ClientId { get; set; }
    public IEnumerable<AuthorizationDetail> AuthorizationDetails { get; set; } = Array.Empty<AuthorizationDetail>();
    public IEnumerable<string> Claims { get; set; } = Array.Empty<string>();
    public string RedirectUri { get; set; }
    public string State { get; set; }
    public string Nonce { get; set; }
    public string CodeChallengeMethod { get; set; }
    public Base64UrlEncodedString CodeChallenge { get; set; }
    public Grant? RequestedGrant { get; set; }
    public string? GrantManagementAction { get; set; }
    public string? Prompt { get; set; }
    public int? MaxAge { get; set; }
    public Base64UrlEncodedString? DPoPPkh { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Grant? FreshGrant { get; set; }
    public string? Sid { get; set; }
    public bool WasUserReauthenticated { get; set; } = false;
    public bool WasConsentPageShown { get; set; } = false;
    public bool AccessDenied { get; set; } = false;
    public bool HasBeenActivated { get; set; } = false;

    public bool HasExpired() => DateTime.UtcNow >= ExpiresAt;
}
