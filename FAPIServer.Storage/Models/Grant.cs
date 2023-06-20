using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.Models;

/// <summary>
/// Models the user consent
/// </summary>
public class Grant
{
    /// <summary>
    /// The unique identifier of this grant. Necessary for Grant Management.
    /// </summary>
    public string GrantId { get; set; }

    /// <summary>
    /// The unique identifier of user who granted access.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// The unique identifier of client which is authorized by this grant.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// The authorization details.
    /// </summary>
    public ICollection<AuthorizationDetail> AuthorizationDetails { get; set; } = new List<AuthorizationDetail>();

    /// <summary>
    /// The claims to which client is authorized.
    /// </summary>
    public ICollection<string> Claims { get; set; } = new HashSet<string>();

    /// <summary>
    /// When the client was authorized; when the user gave consent.
    /// </summary>
    public DateTime GrantedAt { get; set; }
}
