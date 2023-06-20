using FAPIServer.Models;

namespace FAPIServer.RequestHandling.Contexts;

public class GrantManagementContext
{
    public GrantManagementContext(AccessTokenPayload accessToken, string grantId)
    {
        if (string.IsNullOrEmpty(grantId))
            throw new ArgumentException($"'{nameof(grantId)}' cannot be null or empty.", nameof(grantId));

        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        GrantId = grantId;
    }

    public AccessTokenPayload AccessToken { get; set; }
    public string GrantId { get; set; }
}
