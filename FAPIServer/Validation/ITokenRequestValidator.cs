using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface ITokenRequestValidator
{
    Task<TokenRequestValidationResult> ValidateAsync(TokenRequestValidationContext context, CancellationToken cancellationToken = default);
}
