namespace FAPIServer.Validation.Contexts;

public class GrantManagementValidationContext
{
    public GrantManagementValidationContext(string clientId, string? grantId, string? grantManagementAction)
    {
        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));

        ClientId = clientId;
        GrantId = grantId;
        GrantManagementAction = grantManagementAction;
    }

    public string ClientId { get; set; }
    public string? GrantId { get; set; }
    public string? GrantManagementAction { get; set; }
}
