using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface ICibaRequestValidator
{
    Task<CibaRequestValidationResult> ValidateAsync(CibaRequestValidationContext context, CancellationToken cancellationToken = default);
}
