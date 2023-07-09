using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Contexts;

public class ResourceValidationContext
{
    public ResourceValidationContext(Client client, string? grantManagementAction, string? requestedAuthorizationDetails, string? requestedClaims,
        string grantType, Grant? requestedGrant = null)
    {
        if (string.IsNullOrEmpty(grantType))
            throw new ArgumentException($"'{nameof(grantType)}' cannot be null or empty.", nameof(grantType));

        Client = client ?? throw new ArgumentNullException(nameof(client));
        GrantManagementAction = grantManagementAction;
        RequestedAuthorizationDetails = requestedAuthorizationDetails;
        RequestedClaims = requestedClaims;
        GrantType = grantType;
        RequestedGrant = requestedGrant;
    }

    public Client Client { get; set; }
    public string? GrantManagementAction { get; set; }
    public string? RequestedAuthorizationDetails { get; set; }
    public string? RequestedClaims { get; set; }
    public string GrantType { get; set; }
    public Grant? RequestedGrant { get; set; }
}
