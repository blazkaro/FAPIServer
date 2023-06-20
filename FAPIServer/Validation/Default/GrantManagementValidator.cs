using FAPIServer.Extensions;
using FAPIServer.Storage.Stores;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation.Default;

public class GrantManagementValidator : IGrantManagementValidator
{
    private readonly IGrantStore _grantStore;

    public GrantManagementValidator(IGrantStore grantStore)
    {
        _grantStore = grantStore;
    }

    public async Task<GrantManagementValidationResult> ValidateAsync(GrantManagementValidationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        switch (context.GrantManagementAction)
        {
            case Constants.SupportedGrantManagementActions.Create:
                if (!context.GrantId.IsNullOrEmpty())
                    return new(Error.InvalidRequest, "The 'grant_id' cannot be present when 'grant_management_action'='create'");

                break;

            case Constants.SupportedGrantManagementActions.Merge or Constants.SupportedGrantManagementActions.Replace:
                if (context.GrantId.IsNullOrEmpty())
                    return new(Error.InvalidRequest, $"The 'grant_id' must be present when 'grant_management_action'='{context.GrantManagementAction}'");

                var grant = await _grantStore.FindByGrantIdAndClientId(context.GrantId, context.ClientId, cancellationToken);
                if (grant == null)
                    return new(Error.InvalidGrantId, "The grant not found");

                return new(grant);

            default:
                if (!context.GrantManagementAction.IsNullOrEmpty())
                    return new(Error.InvalidRequest, ErrorDescriptions.NotSupportedValue(context.GrantManagementAction, "grant_management_action"));

                if (!context.GrantId.IsNullOrEmpty())
                    return new(Error.InvalidRequest, "The 'grant_id' cannot be present without specified 'grant_management_action'");

                break;
        }

        return new() { IsValid = true };
    }
}
