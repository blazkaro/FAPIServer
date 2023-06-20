using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.Models;

/// <summary>
/// Models the FAPI client
/// </summary>
public class Client
{
    /// <summary>
    /// The unique identifier of the client
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Whether client is enabled (defaults to <c>true</c>)
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Ed25519 public key used to authenticate this client
    /// </summary>
    public Ed25519Key Ed25519PublicKey { get; set; }

    /// <summary>
    /// Whether client must have user consent to authorize. Defaults to <c>true</c>
    /// </summary>
    public bool ConsentRequired { get; set; } = true;

    /// <summary>
    /// Whether client must use Authorization Code Binding to DPoP Key
    /// </summary>
    public bool AuthorizationCodeBindingToDpopKeyRequired { get; set; } = false;

    /// <summary>
    /// Set lifetime for request URI. Defaults to 60s / 1min
    /// </summary>
    public Lifetime RequestUriLifetime { get; set; } = new Lifetime(60);

    /// <summary>
    /// Set lifetime for authorization code. Defaults to 30s / 0.5min
    /// </summary>
    public Lifetime AuthorizationCodeLifetime { get; set; } = new Lifetime(30);

    /// <summary>
    /// Set lifetime for identity token. Defaults to 30s / 0.5min
    /// </summary>
    public Lifetime IdTokenLifetime { get; set; } = new Lifetime(30);

    /// <summary>
    /// Set lifetime for access token. Defaults to 300s / 5min
    /// </summary>
    public Lifetime AccessTokenLifetime { get; set; } = new Lifetime(300);

    /// <summary>
    /// Set lifetime for refresh token. Defaults to 1209600s / 14days
    /// </summary>
    public Lifetime RefreshTokenLifetime { get; set; } = new Lifetime(1209600);

    /// <summary>
    /// Set lifetime for backchannel authentication request. Defaults to 300s / 5min
    /// </summary>
    public Lifetime CibaRequestLifetime { get; set; } = new Lifetime(300);

    /// <summary>
    /// Whether refresh token lifetime is sliding. If set to <c>false</c>, the refresh token lifetime will not change while using `refresh_token` grant at token endpoint.
    /// Otherwise, refresh token lifetime will be set to UTC now + <see cref="RefreshTokenLifetime"/>
    /// </summary>
    public bool SlidingRefreshToken { get; set; } = false;

    /// <summary>
    /// Collection of client's allowed grant types
    /// </summary>
    public IEnumerable<string> AllowedGrantTypes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Collection of client's redirect URIs
    /// </summary>
    public IEnumerable<Uri> RedirectUris { get; set; } = Array.Empty<Uri>();

    /// <summary>
    /// Collection of user claims that client can request during authorization
    /// </summary>
    public IEnumerable<string> AllowedClaims { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Collection of authorization details types that client can request during authorization
    /// </summary>
    public IEnumerable<ClientAllowedSchema> AllowedSchemas { get; set; } = Array.Empty<ClientAllowedSchema>();

    /// <summary>
    /// The human friendly client's name
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// The URI to the client logo
    /// </summary>
    public Uri? LogoUri { get; set; }
}
