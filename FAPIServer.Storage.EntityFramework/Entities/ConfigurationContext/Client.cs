using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Storage.EntityFramework.Entities.ConfigurationContext;

public class Client
{
    public int Id { get; set; }
    public string ClientId { get; set; }
    public bool Enabled { get; set; }
    public Ed25519Key Ed25519PublicKey { get; set; }
    public bool ConsentRequired { get; set; }
    public bool AuthorizationCodeBindingToDpopKeyRequired { get; set; }
    public string? CibaMode { get; set; }
    public Uri? BackchannelClientNotificationEndpoint { get; set; }
    public Lifetime RequestUriLifetime { get; set; }
    public Lifetime AuthorizationCodeLifetime { get; set; }
    public Lifetime IdTokenLifetime { get; set; }
    public Lifetime AccessTokenLifetime { get; set; }
    public Lifetime RefreshTokenLifetime { get; set; }
    public Lifetime CibaRequestLifetime { get; set; }
    public bool SlidingRefreshToken { get; set; }
    public ICollection<ClientGrantType> GrantTypes { get; set; }
    public ICollection<ClientRedirectUri> RedirectUris { get; set; }
    public ICollection<UserClaim> Claims { get; set; }
    public IEnumerable<AuthorizationDetailSchemaAction> AuthorizationDetailSchemaActions { get; set; }
    public string DisplayName { get; set; }
    public Uri? LogoUri { get; set; }
}
