using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IResourceValidator
{
    Task<ResourceValidationResult> ValidateAsync(ResourceValidationContext context, CancellationToken cancellationToken = default);
}
