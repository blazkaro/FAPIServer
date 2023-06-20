using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IGrantManagementValidator
{
    Task<GrantManagementValidationResult> ValidateAsync(GrantManagementValidationContext context, CancellationToken cancellationToken = default);
}
