using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Contexts;

public class ResourceValidationContext
{
    public ResourceValidationContext(Client client, string? grantManagementAction, string? requestedAuthorizationDetails, string? requestedClaims,
        Grant? requestedGrant = null)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        GrantManagementAction = grantManagementAction;
        RequestedAuthorizationDetails = requestedAuthorizationDetails;
        RequestedClaims = requestedClaims;
        RequestedGrant = requestedGrant;
    }

    public Client Client { get; set; }
    public string? GrantManagementAction { get; set; }
    public string? RequestedAuthorizationDetails { get; set; }
    public string? RequestedClaims { get; set; }
    public Grant? RequestedGrant { get; set; }
}
